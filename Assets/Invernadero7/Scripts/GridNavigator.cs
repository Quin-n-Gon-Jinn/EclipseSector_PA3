using System.Collections.Generic;
using UnityEngine;

namespace Invernadero7
{
    // Cached navigation grid. Only static obstacles (layer 8) are sampled, once.
    // A* is requested at most twice per second by the maintenance robot.
    public sealed class GridNavigator
    {
        const int Width=13, Height=23;
        const float Step=2;
        readonly bool[] walkable=new bool[Width*Height];
        public GridNavigator()
        {
            Physics.SyncTransforms();
            for(int i=0;i<walkable.Length;i++) walkable[i]=!Physics.CheckSphere(Position(i)+Vector3.up,.63f,1<<8,QueryTriggerInteraction.Ignore);
        }
        Vector3 Position(int i) => new Vector3((i%Width-6)*Step,0,(i/Width-11)*Step);
        int Nearest(Vector3 point)
        {
            int best=-1; float distance=float.PositiveInfinity;
            for(int i=0;i<walkable.Length;i++)
            {
                if(!walkable[i]) continue;
                float d=(Position(i)-new Vector3(point.x,0,point.z)).sqrMagnitude;
                if(d<distance) { distance=d; best=i; }
            }
            return best;
        }
        public List<Vector3> Find(Vector3 start,Vector3 target)
        {
            int first=Nearest(start), last=Nearest(target), count=walkable.Length;
            var result=new List<Vector3>(); if(first<0 || last<0) return result;
            float[] cost=new float[count]; int[] parent=new int[count]; bool[] closed=new bool[count];
            for(int i=0;i<count;i++) { cost[i]=float.PositiveInfinity; parent[i]=-1; }
            var open=new List<int>{first}; cost[first]=0;
            while(open.Count>0)
            {
                int current=open[0]; float score=float.PositiveInfinity;
                foreach(int node in open)
                {
                    float f=cost[node]+Mathf.Abs(node%Width-last%Width)+Mathf.Abs(node/Width-last/Width);
                    if(f<score) { score=f; current=node; }
                }
                open.Remove(current); if(current==last) break; closed[current]=true;
                int x=current%Width,z=current/Width;
                int[] candidates={x>0?current-1:-1,x<Width-1?current+1:-1,z>0?current-Width:-1,z<Height-1?current+Width:-1};
                foreach(int next in candidates)
                {
                    if(next<0 || !walkable[next] || closed[next] || cost[current]+1>=cost[next]) continue;
                    cost[next]=cost[current]+1; parent[next]=current;
                    if(!open.Contains(next)) open.Add(next);
                }
            }
            if(first!=last && parent[last]<0) return result;
            int cursor=last;
            while(cursor!=first) { result.Add(Position(cursor)); cursor=parent[cursor]; if(cursor<0) break; }
            result.Reverse();
            if(result.Count==0) result.Add(Position(last));
            return result;
        }
    }
}
