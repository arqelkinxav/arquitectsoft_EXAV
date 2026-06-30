// Liquid-glass refraction shader (Pixel Shader 2.0, acelerado por GPU).
// Distorsiona el fondo capturado: dobla la luz hacia los bordes (lente) con un
// leve corrimiento cromático y un realce de filo. Inspirado en la técnica de
// captura+shader del repo MIT "WPF-Liquid-Glass-Effect" (dragosniamtu) y en el
// artículo de AmirHossein Aghajari; shader reescrito desde cero.

sampler2D Input : register(s0);
float EdgeStart : register(c0);   // 0..1: dónde empieza la curvatura (≈0.55)
float Strength  : register(c1);   // intensidad del doblez (≈0.06)
float Rim       : register(c2);   // realce de luz en el filo (≈0.20)

float4 main(float2 uv : TEXCOORD) : COLOR
{
    float2 d = uv - 0.5;
    float len = length(d);
    float r = len * 2.0;                                   // 0 centro .. ~1 bordes
    float t = saturate((r - EdgeStart) / max(1.0 - EdgeStart, 0.0001));
    t = t * t;                                             // suaviza el arranque
    float2 dir = d / (len + 0.0001);
    float2 off = dir * t * Strength;                       // doblez hacia el centro (lupa)

    // Muestrear HACIA ADENTRO (uv - off) nunca se sale de la imagen, así se evita
    // el estirón/clamp en el borde. saturate() es una red de seguridad extra.
    float4 col;
    col.r = tex2D(Input, saturate(uv - off * 1.06)).r;   // aberración cromática sutil
    col.g = tex2D(Input, saturate(uv - off)).g;
    col.b = tex2D(Input, saturate(uv - off * 0.94)).b;
    col.a = 1.0;
    col.rgb += Rim * t;                                    // brillo de filo
    return col;
}
