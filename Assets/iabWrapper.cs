using UnityEngine;
 
using System.Collections;
using System.Collections.Generic;
 
public class iabWrapper : MonoBehaviour 
{
    public delegate void cbFunc(object[] retarr);
    cbFunc iabSetupCB =null;
    cbFunc iabPurchaseCB =null;
    cbFunc iabConsumeCB =null;
 
    AndroidJavaObject mIABHelperObj =null;
    static iabWrapper g_inst =null;
 
    void Start(){
        g_inst =this;
    }
 
    static public void init(string base64EncodedPublicKey, cbFunc tmpIabSetupCBFunc)
    {
        if (g_inst ==null)
            return;
 
        g_inst.iabSetupCB =tmpIabSetupCBFunc;
 
        dispose();
        g_inst.mIABHelperObj =new AndroidJavaObject("com.macaronics.iab.iabWrapper", new object[2]{base64EncodedPublicKey, "iabWrapper"});
    }
 
    static public void dispose()
    {
        if (g_inst ==null)
            return;
 
        if (g_inst.mIABHelperObj !=null)
        {
            g_inst.mIABHelperObj.Call("dispose");
            g_inst.mIABHelperObj.Dispose();
            g_inst.mIABHelperObj =null;
        }
    }

    static public void purchase(string strSKU, int reqCode, string payload, cbFunc tmpIabPurchaseCBFunc)
    {
        if (g_inst ==null)
            return;
     
        g_inst.iabPurchaseCB =tmpIabPurchaseCBFunc;
     
        if (g_inst.mIABHelperObj !=null)
        {
            g_inst.mIABHelperObj.Call("purchase", new object[3]{strSKU, reqCode.ToString(), payload});
        }
    }
     
    static public void consume_inapp(string strPurchaseJsonInfo, string strSignature, cbFunc tmpIabConsumeCBFunc)
    {
        if (g_inst ==null)
            return;
     
        g_inst.iabConsumeCB =tmpIabConsumeCBFunc;
     
        if (g_inst.mIABHelperObj !=null)
        {
            g_inst.mIABHelperObj.Call("consume", new object[3]{"inapp", strPurchaseJsonInfo, strSignature});
        }
    }

    void msgReceiver(string msg)
    {
        if (g_inst ==null)
            return;
        //parse json
        Dictionary<string, object> cache =(Dictionary<string, object>)MiniJSON.Json.Deserialize(msg);
     
        //dispatch msg
        if (cache.ContainsKey("code")==true){
            int val =0;
            int.TryParse((string)cache["code"], out val);
            switch(val){
                case 0:{
                    //unknown
                    Debug.Log("Unity-iabWrappe :cannot parse cache[code]");
     
                }
                break;
     
                case 1:{
                    //OnIabSetupFinishedListener
                    if (cache.ContainsKey("ret")==true){
                        string retval =(string)cache["ret"];
                        if (retval =="true"){
                            //可使用
                            if (iabSetupCB !=null)
                            {
                                iabSetupCB( new object[1]{true} );
                            }
     
                        }
                        else if (retval =="false"){
                            //不可使用
                            if (iabSetupCB !=null)
                            {
                                iabSetupCB( new object[1]{false} );
                            }
     
                        }else{
                            Debug.Log("Unity-iabWrapper :cannot parse cache[ret], code=1");
                        }
                    }
                }
                break;
     
                case 2:{
                    //onIabPurchaseFinished
                    if (cache.ContainsKey("ret")==true){
                        string retval =(string)cache["ret"];
                        if (retval =="true"){
                            //可使用
                            if (iabPurchaseCB !=null)
                            {
                                iabPurchaseCB( new object[3]{true, (string)cache["desc"], (string)cache["sign"]} );
                            }
     
                        }
                        else if (retval =="false"){
                            //不可使用
                            if (iabPurchaseCB !=null)
                            {
                                iabPurchaseCB( new object[3]{false, "", ""} );
                            }
     
                        }
                        else{
                            Debug.Log("Unity-iabWrapper  :cannot parse cache[ret], code=2");
     
                        }
                    }
                }
                break;
     
                case 3:{
                    //OnConsumeFinishedListener
                    if (cache.ContainsKey("ret")==true){
                        string retval =(string)cache["ret"];
                        if (retval =="true"){
                            //可使用
                            if (iabConsumeCB !=null)
                            {
                                iabConsumeCB( new object[3]{true, (string)cache["desc"], (string)cache["sign"]} );
                            }
     
                        }else if (retval =="false"){
                            //不可使用
                            if (iabConsumeCB !=null)
                            {
                                iabConsumeCB( new object[3]{false, "", ""} );
                            }
     
                        }
                        else{
                            Debug.Log("Unity-iabWrapper :cannot parse cache[ret], code=3");
     
                        }
                    }
                }
                break;
            }
        }
    }

}