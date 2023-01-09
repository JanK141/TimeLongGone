#ifndef CUSTOM_LIGHTING_INCLUDED
#define CUSTOM_LIGHTING_INCLUDED
#define MAIN_LIGHT_CALCULATE_SHADOWS
#define _MAIN_LIGHT_SHADOWS_CASCADE
#define _SHADOWS_SOFT
#define ADDITIONAL_LIGHT_CALCULATE_SHADOWS

void CalculateMainLight_float(in float3 WorldPos, out float3 Direction, out float3 Color, out half DistanceAtten, out half ShadowAtten) {
#ifdef SHADERGRAPH_PREVIEW
    Direction = half3(0.5, 0.5, 0);
    Color = 1;
    DistanceAtten = 1;
    ShadowAtten = 1;
#else
#if SHADOWS_SCREEN
    half4 clipPos = TransformWorldToHClip(WorldPos);
    half4 shadowCoord = ComputeScreenPos(clipPos);
#else
    half4 shadowCoord = TransformWorldToShadowCoord(WorldPos);
#endif
    Light mainLight = GetMainLight(shadowCoord);
    Direction = mainLight.direction;
    Color = mainLight.color;
    DistanceAtten = mainLight.distanceAttenuation;
    ShadowAtten = mainLight.shadowAttenuation;
#endif
}

float GetLightIntensity(in float3 color) {
    return max(color.r, max(color.g, color.b));
}

#ifndef SHADERGRAPH_PREVIEW
Light GetAdditionalLightForToon(in int pixelLightIndex, in float3 worldPosition) {
    int perObjectLightIndex = GetPerObjectLightIndex(pixelLightIndex);
    Light light = GetAdditionalPerObjectLight(perObjectLightIndex, worldPosition);
    light.shadowAttenuation = AdditionalLightRealtimeShadow(perObjectLightIndex, worldPosition);
    return light;
}
#endif

void AddAdditionalLights_float(in float Smoothness, in float3 WorldPosition, in float3 WorldNormal, in float3 WorldView,
    in float MainDiffuse, in float MainSpecular, in float3 MainColor, in float3 MainDirection,
    out float Diffuse, out float Specular, out float3 Color, out float3 AvgDirection) {

    float mainIntensity = GetLightIntensity(MainColor);
    Diffuse = MainDiffuse * mainIntensity;
    Specular = MainSpecular * mainIntensity;
    Color = MainColor *(MainDiffuse + MainSpecular);
    AvgDirection = MainDirection;

#ifndef SHADERGRAPH_PREVIEW

    int pixelLightCount = GetAdditionalLightsCount();
    for (int i = 0; i < pixelLightCount; ++i) {
        Light light = GetAdditionalLightForToon(i, WorldPosition);
        half NdotL = saturate(dot(WorldNormal, light.direction));
        half atten = light.distanceAttenuation * light.shadowAttenuation * GetLightIntensity(light.color);
        half thisDiffuse = atten * NdotL;
        half thisSpecular = LightingSpecular(thisDiffuse, light.direction, WorldNormal, WorldView, 1, Smoothness);
        Diffuse += thisDiffuse;
        Specular += thisSpecular;
        AvgDirection += light.direction;

        Color += light.color * (thisDiffuse + thisSpecular);
    }
    AvgDirection /= 1 + pixelLightCount;
#endif
half total = Diffuse + Specular;
Color = total <= 0 ? MainColor : Color / total;
}


#endif