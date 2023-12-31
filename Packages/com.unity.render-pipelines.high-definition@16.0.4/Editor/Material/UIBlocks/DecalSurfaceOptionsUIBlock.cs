using UnityEngine;
using UnityEngine.Rendering.HighDefinition;

// Include material common properties names
using static UnityEngine.Rendering.HighDefinition.HDMaterialProperties;

namespace UnityEditor.Rendering.HighDefinition
{
    /// <summary>The UI block that represents the surface options for decal materials.</summary>
    public class DecalSurfaceOptionsUIBlock : MaterialUIBlock
    {
        internal class Styles
        {
            public static GUIContent header { get; } = EditorGUIUtility.TrTextContent("Surface Options");
            public static GUIContent affectAlbedoText = new GUIContent("Affect BaseColor", "When enabled, this decal uses its base color. When disabled, the decal has no base color effect.");
            public static GUIContent affectNormalText = new GUIContent("Affect Normal", "When enabled, this decal uses its normal. When disabled, the decal has nonormal effect.");
            public static GUIContent affectMetalText = new GUIContent("Affect Metal", "When enabled, this decal uses the metallic channel of its Mask Map. When disabled, the decal has no metallic effect.");
            public static GUIContent affectAmbientOcclusionText = new GUIContent("Affect Ambient Occlusion", "When enabled, this decal uses the ambient occlusion channel of its Mask Map. When disabled, the decal has no ambient occlusion effect.");
            public static GUIContent affectSmoothnessText = new GUIContent("Affect Smoothness", "When enabled, this decal uses the smoothness channel of its Mask Map. When disabled, the decal has no smoothness effect.");
            public static GUIContent affectEmissionText = new GUIContent("Affect Emission", "When enabled, this decal becomes emissive and appears self-illuminated. Affect Emission does not support Affects Transparents option on Decal Projector.");
            public static GUIContent supportLodCrossFadeText = new GUIContent("Support LOD CrossFade", "When enabled, this decal material supports LOD Cross fade if use on a Mesh.");
            public static GUIContent transparentDynamicUpdateText = new GUIContent("Transparent Dynamic Update", "When enabled, the decal will update the transparent decal atlas every frame. This has a performance impact.");
        }

        MaterialProperty affectsAlbedo = new MaterialProperty();
        MaterialProperty affectsNormal = new MaterialProperty();
        MaterialProperty affectsMetal = new MaterialProperty();
        MaterialProperty affectsAO = new MaterialProperty();
        MaterialProperty affectsSmoothness = new MaterialProperty();
        MaterialProperty affectsEmission = new MaterialProperty();
        MaterialProperty transparentDynamicUpdate = new MaterialProperty();

        /// <summary>
        /// Constructs a DecalSurfaceOptionsUIBlock based on the parameters.
        /// </summary>
        /// <param name="expandableBit">Bit index used to store the foldout state.</param>
        public DecalSurfaceOptionsUIBlock(ExpandableBit expandableBit)
            : base(expandableBit, Styles.header)
        {
        }

        /// <summary>
        /// Loads the material properties for the block.
        /// </summary>
        public override void LoadMaterialProperties()
        {
            affectsAlbedo = FindProperty(kAffectAlbedo);
            affectsNormal = FindProperty(kAffectNormal);
            affectsMetal = FindProperty(kAffectMetal);
            affectsAO = FindProperty(kAffectAO);
            affectsSmoothness = FindProperty(kAffectSmoothness);
            affectsEmission = FindProperty(kAffectEmission);
            transparentDynamicUpdate = FindProperty(kTransparentDynamicUpdateDecals);
        }

        /// <summary>
        /// Renders the properties in the block.
        /// </summary>
        protected override void OnGUIOpen()
        {
            bool perChannelMask = false;
            HDRenderPipelineAsset hdrp = HDRenderPipeline.currentAsset;
            if (hdrp != null)
            {
                perChannelMask = hdrp.currentPlatformRenderPipelineSettings.decalSettings.perChannelMask;
            }

            if (affectsAlbedo != null)
                materialEditor.ShaderProperty(affectsAlbedo, Styles.affectAlbedoText);
            if (affectsNormal != null)
                materialEditor.ShaderProperty(affectsNormal, Styles.affectNormalText);

            using (new EditorGUI.DisabledScope(!perChannelMask))
            {
                if (affectsMetal != null)
                    materialEditor.ShaderProperty(affectsMetal, Styles.affectMetalText);
                if (affectsAO != null)
                    materialEditor.ShaderProperty(affectsAO, Styles.affectAmbientOcclusionText);
            }

            if (affectsSmoothness != null)
                materialEditor.ShaderProperty(affectsSmoothness, Styles.affectSmoothnessText);

            if (affectsEmission != null)
                materialEditor.ShaderProperty(affectsEmission, Styles.affectEmissionText);

            if (!perChannelMask && (affectsMetal != null || affectsAO != null))
            {
                EditorGUILayout.HelpBox("Enable 'Metal and AO properties' in your HDRP Asset if you want to control the Metal and AO properties of decals. There is a performance cost of enabling this option.",
                    MessageType.Info);
            }

            if (transparentDynamicUpdate != null)
                materialEditor.ShaderProperty(transparentDynamicUpdate, Styles.transparentDynamicUpdateText);
        }
    }
}
