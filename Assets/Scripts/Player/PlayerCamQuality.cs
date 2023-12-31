using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.HighDefinition;
using UnityEngine.Rendering;

public class PlayerCamQuality : MonoBehaviour
{
    public void Start()
    {
        //Objects
        HDAdditionalCameraData camData = this.GetComponent<HDAdditionalCameraData>();
        VolumeProfile volume = this.GetComponent<Volume>().profile;
        GlobalIllumination ssgi;
        ScreenSpaceReflection ssr;

        //Tryget
        volume.TryGet<GlobalIllumination>(out ssgi);
        volume.TryGet<ScreenSpaceReflection>(out ssr);

        //Frame Setup
        FrameSettings frameSettings = camData.renderingPathCustomFrameSettings;
        FrameSettingsOverrideMask frameSettingsOverrideMask = camData.renderingPathCustomFrameSettingsOverrideMask;
        camData.customRenderingSettings = true;

        //SSAO
        if (ssr != null)
        {
            int screenSpaceReflections = PlayerPrefs.GetInt("Settings: Reflections");
            if (screenSpaceReflections == 0)
            {
                ssr.tracing.value = RayCastingMode.RayTracing;
                ssr.mode.value = RayTracingMode.Performance;
            }
            else
            {
                ssr.tracing.value = RayCastingMode.RayTracing;
                ssr.mode.value = RayTracingMode.Quality;
            }
        }

        //SSGI
        if (ssgi != null)
        {
            int globalIlluminationSolution = PlayerPrefs.GetInt("Settings: Lighting");
            if (globalIlluminationSolution == 0)
            {
                ssgi.tracing.value = RayCastingMode.RayMarching;
            }
            else
            {
                ssgi.tracing.value = RayCastingMode.RayTracing;
                if (globalIlluminationSolution == 1)
                {
                    ssgi.mode.value = RayTracingMode.Performance;
                }
                else
                {
                    ssgi.mode.value = RayTracingMode.Quality;
                }

                //Bounces & Samples
                ssgi.bounceCount.value = PlayerPrefs.GetInt("Settings: Bounce Count");
                ssgi.sampleCount.value = PlayerPrefs.GetInt("Settings: Sample Count");
            }
        }

        //DLSS
        int dlss = PlayerPrefs.GetInt("Settings: DLSS");
        if (dlss == 0)
        {
            camData.allowDeepLearningSuperSampling = false;
        }
        else
        {
            camData.allowDeepLearningSuperSampling = true;
            camData.deepLearningSuperSamplingUseCustomQualitySettings = true;
            switch (dlss)
            {
                case 1:
                    camData.deepLearningSuperSamplingQuality = 3;
                    break;
                case 2:
                    camData.deepLearningSuperSamplingQuality = 0;
                    break;
                case 3:
                    camData.deepLearningSuperSamplingQuality = 1;
                    break;
                case 4:
                    camData.deepLearningSuperSamplingQuality = 2;
                    break;
                default:
                    break;
            }
        }

        //Applying the frame setting mask back to the camera
        camData.renderingPathCustomFrameSettingsOverrideMask = frameSettingsOverrideMask;
    }

}
