#include "Packages/com.unity.render-pipelines.core/ShaderLibrary\CommonMaterial.hlsl"
#include "Packages/com.unity.render-pipelines.high-definition/Runtime/RenderPipeline/Raytracing/Shaders/RaytracingFragInputs.hlsl"

// Generic function that handles the reflection code
[shader("closesthit")]
void ClosestHitVisibility(inout RayIntersectionVisibility rayIntersection : SV_RayPayload, AttributeData attributeData : SV_IntersectionAttributes)
{
    UNITY_XR_ASSIGN_VIEW_INDEX(DispatchRaysIndex().z);

    IntersectionVertex currentVertex;
    FragInputs fragInput;
    uint currentFrameIndex = GetCurrentVertexAndBuildFragInputs(attributeData, currentVertex, fragInput);

    // Compute the distance of the ray
    rayIntersection.t = RayTCurrent();

    // Compute the velocity of the itnersection
    #ifdef HAVE_VFX_MODIFICATION
        uint index, instanceIndex, instanceActiveIndex;
        GetVFXInstancingIndices(index, instanceIndex, instanceActiveIndex);
        float3 inputVertexPosition = 0.0;
        rayIntersection.velocity = GetVFXVertexDisplacement(index, fragInput.positionRWS, inputVertexPosition, currentFrameIndex);
    #else
        float3 positionOS = ObjectRayOrigin() + ObjectRayDirection() * rayIntersection.t;
        float3 previousPositionWS = TransformPreviousObjectToWorld(positionOS);
        rayIntersection.velocity = saturate(length(previousPositionWS - fragInput.positionRWS));
    #endif
}

// Generic function that handles the reflection code
[shader("anyhit")]
void AnyHitVisibility(inout RayIntersectionVisibility rayIntersection : SV_RayPayload, AttributeData attributeData : SV_IntersectionAttributes)
{
    UNITY_XR_ASSIGN_VIEW_INDEX(DispatchRaysIndex().z);

    IntersectionVertex currentVertex;
    FragInputs fragInput;
    uint currentFrameIndex = GetCurrentVertexAndBuildFragInputs(attributeData, currentVertex, fragInput);

    // Compute the view vector
    float3 viewWS = -WorldRayDirection();

    // Compute the distance of the ray
    rayIntersection.t = RayTCurrent();

    PositionInputs posInput;
    posInput.positionWS = fragInput.positionRWS;
    posInput.positionSS = rayIntersection.pixelCoord;

    // Build the surfacedata and builtindata
    SurfaceData surfaceData;
    BuiltinData builtinData;
    bool isVisible;
    GetSurfaceAndBuiltinData(fragInput, viewWS, posInput, surfaceData, builtinData, currentVertex, rayIntersection.cone, isVisible);

    // If this point is not visible, ignore the hit and force end the shader
    if (!isVisible)
    {
        IgnoreHit();
        return;
    }

#if defined(TRANSPARENT_COLOR_SHADOW) && defined(_SURFACE_TYPE_TRANSPARENT)
    // Compute the velocity of the itnersection

    #ifdef HAVE_VFX_MODIFICATION
        uint index, instanceIndex, instanceActiveIndex;
        GetVFXInstancingIndices(index, instanceIndex, instanceActiveIndex);
        float3 inputVertexPosition = 0.0;
        rayIntersection.velocity = GetVFXVertexDisplacement(index, posInput.positionWS, inputVertexPosition, currentFrameIndex);
    #else
        float3 positionOS = ObjectRayOrigin() + ObjectRayDirection() * rayIntersection.t;
        float3 previousPositionWS = TransformPreviousObjectToWorld(positionOS);
        rayIntersection.velocity = saturate(length(previousPositionWS - fragInput.positionRWS));
    #endif

    // Adjust the color based on the transmittance or opacity
    #if HAS_REFRACTION
        rayIntersection.color *= lerp(surfaceData.transmittanceColor, float3(0.0, 0.0, 0.0), 1.0 - surfaceData.transmittanceMask);
    #else
        rayIntersection.color *= (1.0 - builtinData.opacity);
    #endif

    // Ignore to move to the following intersections
    IgnoreHit();
#else
    else
    {
        // If this fella is opaque, then we need to stop
        rayIntersection.color = float3(0.0, 0.0, 0.0);
        AcceptHitAndEndSearch();
    }
#endif
}
