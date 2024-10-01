using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ani_Controller : MonoBehaviour
{
    Animator ani;
    public GameObject umbrella;
    public GameObject girl;
    string ani_1;
    string ani_2;
    string ani_3;
    string ani_4;
    string ani_5;
    string ani_6;
    void Start()
    {
        ani = girl.transform.GetComponent<Animator>();
        ani_1 = "isTPose";
        ani_2 = "isKiss";
        ani_3 = "isIdle";
        ani_4 = "isWalk";
        ani_5 = "isGrog";

        umbrella.SetActive(false);
    }

    void Ani_Reset()
    {
        ani.SetBool(ani_1, false);
        ani.SetBool(ani_2, false);
        ani.SetBool(ani_3, false);
        ani.SetBool(ani_4, false);
        ani.SetBool(ani_5, false);
    }

    public void Btn_1()
    {
        Ani_Reset();
        ani.SetBool(ani_1, true);
        umbrella.SetActive(false);
        girl.transform.eulerAngles = new Vector3(0, 180, 0);
    }
    public void Btn_2()
    {
        Ani_Reset();
        ani.SetBool(ani_2, true);
        umbrella.SetActive(true);
        girl.transform.eulerAngles = new Vector3(0, 130, 0);
    }
    public void Btn_3()
    {
        Ani_Reset();
        ani.SetBool(ani_3, true);
        umbrella.SetActive(true);
        girl.transform.eulerAngles = new Vector3(0, 180, 0);
    }
    public void Btn_4()
    {
        Ani_Reset();
        ani.SetBool(ani_4, true);
        umbrella.SetActive(true);
        girl.transform.eulerAngles = new Vector3(0, 180, 0);
    }

    public void Btn_5()
    {
        Ani_Reset();
        ani.SetBool(ani_5, true);
        umbrella.SetActive(false);
        girl.transform.eulerAngles = new Vector3(0, 220, 0);
    }

}
