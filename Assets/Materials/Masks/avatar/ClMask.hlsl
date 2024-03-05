// Custom.hlsl

#ifndef CLMASK
#define CLMASK

void ClMask_float(float4 UV, out float Alpha)
{
    float lx = (UV.x - 0.5) * 2.0;
    float ly = (UV.y - 0.5) * 2.0;

    float d = sqrt(lx * lx + ly * ly);
    
    if(d > 1)Alpha = 0;
    else
    Alpha = 1;
    
}

#endif