using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class PlayerNetwork : NetworkBehaviour{

    public GameObject playerPrefab;
    
    void Start(){
        if(!isLocalPlayer){
            return;
        }
        
        CmdSpawnMyUnit();
    }
    
    void Update(){
        
    }
    
    [Command]
    void CmdSpawnMyUnit(){
        GameObject go = Instantiate(playerPrefab);
        
        NetworkServer.SpawnWithClientAuthority(go, connectionToClient);
    }

}
