using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColorChange : MonoBehaviour {

    public Texture columnTextureGreen;
    public Texture columnTextureRed;


    public void SetColor(int c)
    {
        transform.GetComponent<NetworkView>().RPC("SetColorRPC", RPCMode.AllBuffered, c);
    }

    [RPC]
    void SetColorRPC(int c)
    {
        
        Renderer rend = transform.GetComponent<Renderer>();

        if (c == 1)

            rend.material.mainTexture = columnTextureGreen;

        else

            rend.material.mainTexture = columnTextureRed;
            
    }
}
