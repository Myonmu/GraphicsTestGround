/* osParams layout
 * x: min os y pos (anchor)
 * y: max os y pos
 * z: 
 * w:
 */
void PlantVertexDisplace_float(
    in float3 osPos,
    in float3 wsPos,
    in float4 osParams,
    in float4 influenceTexA,
    in float4 influenceTexB,
    in float trample,
    out float3 osDisplacement,
    out float trampleStr
)
{
    //if (influenceTexA.w < 0.1) return;
    osDisplacement = 0;
    trampleStr = 0;
    osPos.y -= osParams.x;
    float2 dir = influenceTexA.xy;
    float h = influenceTexA.z;
    // determine press strength (reduce strength when the object is above the grass)
    float rawStr = 1 - (h - wsPos.y)/max(0.1,osParams.y);
    float str = smoothstep(0, 0.05, rawStr);
    
    float angle = max(0.001,influenceTexA.w);
    float dist = max(0.0001,length(dir));
    float3 d = normalize(float3(dir.x, 0.001, dir.y));
    float3 n = cross(d, float3(0,1,0));
    n.x = max(n.x, 0.01);
    n = normalize(n);
    float3 planarDisplacement = float3(osPos.x, 0, osPos.z);
    float3 vFromRoot = osPos - planarDisplacement;
    /* Instead of lateral displacement, we use rotation.
     * Rotation can better preserve the shape of the grass.
     */
    // Rodrigue rotation
    float3 afterRot = vFromRoot * cos(angle) + cross(n, vFromRoot)* sin(angle) + n*dot(n,vFromRoot)*(1-cos(angle));
    // Less influence closer to the root
    float normalizedHeight = saturate((osPos.y-osParams.x)/max(0.1, osParams.y-osParams.x));
    float trampleLen = - ( 1 - dist ) * vFromRoot.y * 1.5 * trample;
    osDisplacement = (vFromRoot - afterRot) * normalizedHeight + float3(0,trampleLen,0);
    osDisplacement *= str;
    trampleStr = saturate(rawStr) *  (1 - dist) * trample;
}