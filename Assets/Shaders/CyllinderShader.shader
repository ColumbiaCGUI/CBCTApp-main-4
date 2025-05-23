﻿Shader "Custom/CyllinderShader"
{
    Properties
    {
        _Color ("Color", Color) = (1,1,1,1)
        _MainTex ("Albedo (RGB)", 2D) = "white" {}
        _Glossiness ("Smoothness", Range(0,1)) = 0.5
        _Metallic ("Metallic", Range(0,1)) = 0.0
        _AlphaThreshold ("Alpha Threshold", Range(0,1)) = 0.99
        // _ClipTransparency("Clipped Transparency", Range(0,1)) = 0.5
    }
    SubShader
    {
        // Tags { "RenderType"="Transparent" "Queue"="Transparent" }
        // ZWrite Off
        // Blend SrcAlpha OneMinusSrcAlpha

        // // Tags { "RenderType"="Transparent" "Queue"="Transparent" }
        // // LOD 200

        // // Cull Off
        // // ZWrite Off
        // // Blend SrcAlpha OneMinusSrcAlpha

        // // render faces regardless if they point towards the camera or away from it
        // Cull Off

        // CGPROGRAM
        // // Physically based Standard lighting model, and enable shadows on all light types
        // #pragma surface surf Standard fullforwardshadows alpha:fade

        // // Use shader model 3.0 target, to get nicer looking lighting
        // #pragma target 3.0

        // sampler2D _MainTex;

        // struct Input
        // {
        //     float2 uv_MainTex;
        //     float3 worldPos;
        // };

        // half _Glossiness;
        // half _Metallic;
        // fixed4 _Color;

        // float4 _PlaneUp;
        // float4 _PlaneDown;
        // float4 _Cylinder;
        // float3 _Position;
        // // float _ClipTransparency;

        // // Add instancing support for this shader. You need to check 'Enable Instancing' on materials that use the shader.
        // // See https://docs.unity3d.com/Manual/GPUInstancing.html for more information about instancing.
        // // #pragma instancing_options assumeuniformscaling
        // UNITY_INSTANCING_BUFFER_START(Props)
        //     // put more per-instance properties here
        // UNITY_INSTANCING_BUFFER_END(Props)

        // void surf (Input IN, inout SurfaceOutputStandard o)
        // {
        //     //Calculate distance from sphere c = (0,0,0) r = 0.1
        //     /*float3 center = float3(0.0, 0.0, 0.0);
        //     float radius = 0.1;*/

        //     //clip the two planes
        //     float distanceUp = dot(IN.worldPos - _Position, _PlaneUp.xyz);
        //     distanceUp = distanceUp + _PlaneUp.w;

        //     float distanceDown = dot(IN.worldPos - _Position, _PlaneDown.xyz);
        //     distanceDown = distanceDown + _PlaneDown.w;

        //     clip(distanceUp * distanceDown);

        //     // // Calculate combined clip factor (1 = fully visible, 0 = fully clipped)
        //     // float visibility = saturate(sign(distanceUp * distanceDown * cylinderClip));
            
        //     // // Apply standard shading
        //     // fixed4 c = tex2D(_MainTex, IN.uv_MainTex) * _Color;
        //     // o.Albedo = c.rgb;
        //     // o.Metallic = _Metallic;
        //     // o.Smoothness = _Glossiness;

        //     // // Control transparency
        //     // o.Alpha = lerp(_ClipTransparency, c.a, visibility);

        //     //clip the radius
        //     //clip the radius
        //     /**
        //     float3 vectFromCenter = IN.worldPos - _Position - _Cylinder.xyz;
        //     float3 vectFromCenterProj = dot(vectFromCenter, _PlaneUp.xyz) * normalize(_PlaneUp.xyz);
        //     float3 pointRadius = vectFromCenter - vectFromCenterProj;   
        //     float dist = _Cylinder.w*_Cylinder.w - (pointRadius.x*pointRadius.x + pointRadius.y*pointRadius.y + pointRadius.z*pointRadius.z);

        //     clip(dist);

        //     **/
            
        //     // Vector to center of cylinder within the xray head 
        //     float2 cylinder_center = _Cylinder.xz + _Position.xz;
        //     float dist = pow(pow((IN.worldPos.x - cylinder_center.x), 2) + pow((IN.worldPos.z - cylinder_center.y),2), 0.5);
        //     clip(_Cylinder.w - dist);
        




        //     // Default surface shading.
        //     // Albedo comes from a texture tinted by color          :
        //     fixed4 c = tex2D (_MainTex, IN.uv_MainTex) * _Color;
        //     o.Albedo = c.rgb;
        //     // Metallic and smoothness come from slider variables
        //     o.Metallic = _Metallic;
        //     o.Smoothness = _Glossiness;
        //     o.Alpha = c.a;
        // }
        // ENDCG
    // }
    // FallBack "Diffuse"
    // ----- PASS 1: OPAQUE WHEN ALPHA >= THRESHOLD -----
    // First Pass: Opaque (alpha >= threshold)
    Tags { "RenderType"="Opaque" }
    ZWrite On
    Blend One Zero

    CGPROGRAM
    #pragma surface surf Standard fullforwardshadows
    #pragma target 3.0

    sampler2D _MainTex;
    half _Glossiness;
    half _Metallic;
    fixed4 _Color;
    float _AlphaThreshold;

    float4 _PlaneUp;
    float4 _PlaneDown;
    float4 _Cylinder;
    float3 _Position;

    struct Input
    {
        float2 uv_MainTex;
        float3 worldPos;
    };

    void surf(Input IN, inout SurfaceOutputStandard o)
    {
        float distanceUp = dot(IN.worldPos - _Position, _PlaneUp.xyz) + _PlaneUp.w;
        float distanceDown = dot(IN.worldPos - _Position, _PlaneDown.xyz) + _PlaneDown.w;
        clip(distanceUp * distanceDown);

        float2 cylinder_center = _Cylinder.xz + _Position.xz;
        float dist = pow(pow((IN.worldPos.x - cylinder_center.x), 2) + pow((IN.worldPos.z - cylinder_center.y),2), 0.5);
        clip(_Cylinder.w - dist);

        fixed4 c = tex2D(_MainTex, IN.uv_MainTex) * _Color;

        if (c.a < _AlphaThreshold) discard; // skip this pass if not opaque

        o.Albedo = c.rgb;
        o.Metallic = _Metallic;
        o.Smoothness = _Glossiness;
        o.Alpha = c.a;
    }
    ENDCG

    // Second Pass: Transparent (alpha < threshold)
    Tags { "RenderType"="Transparent" "Queue"="Transparent" }
    ZWrite Off
    Blend SrcAlpha OneMinusSrcAlpha

    CGPROGRAM
    #pragma surface surf Standard fullforwardshadows alpha:fade
    #pragma target 3.0

    sampler2D _MainTex;
    half _Glossiness;
    half _Metallic;
    fixed4 _Color;
    float _AlphaThreshold;

    float4 _PlaneUp;
    float4 _PlaneDown;
    float4 _Cylinder;
    float3 _Position;

    struct Input
    {
        float2 uv_MainTex;
        float3 worldPos;
    };

    void surf(Input IN, inout SurfaceOutputStandard o)
    {
        float distanceUp = dot(IN.worldPos - _Position, _PlaneUp.xyz) + _PlaneUp.w;
        float distanceDown = dot(IN.worldPos - _Position, _PlaneDown.xyz) + _PlaneDown.w;
        clip(distanceUp * distanceDown);

        float2 cylinder_center = _Cylinder.xz + _Position.xz;
        float dist = pow(pow((IN.worldPos.x - cylinder_center.x), 2) + pow((IN.worldPos.z - cylinder_center.y),2), 0.5);
        clip(_Cylinder.w - dist);

        fixed4 c = tex2D(_MainTex, IN.uv_MainTex) * _Color;

        if (c.a >= _AlphaThreshold) discard; // skip this pass if opaque

        o.Albedo = c.rgb;
        o.Metallic = _Metallic;
        o.Smoothness = _Glossiness;
        o.Alpha = c.a;
    }
    ENDCG
    }

    FallBack "Diffuse"
}
