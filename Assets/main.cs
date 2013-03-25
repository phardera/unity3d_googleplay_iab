using UnityEngine;
using System.Collections;
 
public class main : MonoBehaviour {
 
    // Use this for initialization
    void Start () {
        iabWrapper.init(
            "PUBLIC_KEY",
            delegate(object[] ret){
                if (true ==(bool)ret[0]){
                    Debug.Log("iab successfully initialized");
                }
                else{
                    Debug.Log("failed to initialize iab");
                }
            });
    }
 
    void OnGUI(){
        if (GUI.Button(new Rect(0, 0, 100, 100), "purchase")){
            iabWrapper.purchase("PRODUCT_SKU", 10001, "PRODUCT_SKU_AND_USER_ID_AND_DATE",
                delegate(object[] ret){
                    if (false ==(bool)ret[0]){
                        Debug.Log("purchase cancelled");
                    }
                    else{
                        string purchaseinfo =(string)ret[1];
                        string signature =(string)ret[2];
                        iabWrapper.consume_inapp(purchaseinfo, signature, 
                            delegate(object[] ret2){
                                if (false ==(bool)ret2[0])
                                {
                                    Debug.Log("failed to consume product");
                                }
                            });
                    }
                });
        }
    }
 
    void OnApplicationQuit(){
        iabWrapper.dispose();
    }
}