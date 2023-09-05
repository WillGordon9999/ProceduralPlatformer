using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Collections;
using Unity.Jobs;

public class OverlapCommandTest : MonoBehaviour
{
    // Start is called before the first frame update
    IEnumerator Start()
    {
        while (SplineLevelCreator.Instance.spawnedObjects.Count < 1)
            yield return null;

        int maxCommands = 1;
        int maxHits = 3;

        NativeArray<OverlapBoxCommand> commands = new NativeArray<OverlapBoxCommand>(maxCommands, Allocator.TempJob);
        NativeArray<ColliderHit> hits = new NativeArray<ColliderHit>(maxHits, Allocator.TempJob);

        commands[0] = new OverlapBoxCommand(Vector3.zero, Vector3.one, Quaternion.identity, QueryParameters.Default);

        //JobHandle handle = OverlapBoxCommand.ScheduleBatch(commands, hits, maxCommands, maxHits);
        //handle.Complete();

        for (int j = 0; j < 10; j++)
        {
            for (int i = 0; i < maxHits; i++)
            {
                if (hits[i].collider != null)
                {
                    Debug.Log($"hit {hits[i].collider.gameObject.name}");
                }

                yield return null;
            }

            JobHandle handle = OverlapBoxCommand.ScheduleBatch(commands, hits, maxCommands, maxHits);
            handle.Complete();
        }

        

        yield return null;

        commands.Dispose();
        hits.Dispose();

        yield return null;

        Debug.Log("Made it to the end of the death");
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
