﻿using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static DataBase;
using static PlayerValue;
using static Manifesto;
using UnityEngine.SceneManagement;
public class UI_Player : UI_State
{
    [SerializeField] Player_Cont PCont;
    [SerializeField] Player_Atk PAtk;
    [SerializeField] Image MPBar;
    [SerializeField] Image MPFill;
    [SerializeField] TextMeshProUGUI AtkFBTx;
    [SerializeField] UI_Sin_Atk[] AtkUIs;
    [SerializeField] TextMeshProUGUI StateInfoTx;
    [SerializeField] BinaryUIAnimationo binaryUIAnimationo;
    [SerializeField] GameObject DebugUI;

    #region 関数

    /// <summary>
    /// ステータスUIを更新するメソッド
    /// </summary>
    /// <param name="index"></param>
    /// <param name="AtkD"></param>
    /// <param name="AtkCTs"></param>
    private void UpdateStatus(int index, Data_Atk AtkD, Class_Sta_AtkCT AtkCTs)
    {
        // AtkUIs[index] の UI 更新処理
        for (int j = 0; j < AtkUIs[j].ChengedImages.Length; j++)
        {
            // すべての変更画像を非表示にする
            AtkUIs[index].ChengedImages[j].SetActive(false);

            // 名前の表示を更新（存在する場合）
            if (AtkUIs[index].Name[j])
            {
                AtkUIs[index].Name[j].text = AtkD.Name;
            }

            // アイコンの表示を更新（存在する場合）
            if (AtkUIs[index].Icon[j])
            {
                AtkUIs[index].Icon[j].texture = AtkD.Icon;
            }

            // クールタイムの画像更新（存在する場合）
            if (AtkUIs[index].CTImage[j])
            {
                if (AtkCTs != null)
                {
                    // クールタイムの進行状況を計算して更新
                    AtkUIs[index].CTImage[j].fillAmount = ((float)AtkCTs.CT / Mathf.Max(1, AtkCTs.CTMax));
                }
                else AtkUIs[index].CTImage[j].fillAmount = 0;
            }

            // チャージ量の画像更新（存在する場合）
            if (AtkUIs[index].ChargeImage[j])
            {
                if (AtkD.SPUse > 0)
                {
                    // スタミナ（SP）の使用量に対する割合を計算して更新
                    AtkUIs[index].ChargeImage[j].fillAmount = (float)Sta.SP / AtkD.SPUse;

                    // スタミナ不足の場合、背景を灰色に変更
                    //if (Sta.SP < AtkD.SPUse && AtkUIs[index].BackImage[j])
                    //{
                    //    AtkUIs[index].BackImage[j].color = Color.gray;
                    //}
                }
                else AtkUIs[index].ChargeImage[j].fillAmount = 0;
            }
            AtkUIs[index].FullImage.SetActive(AtkD.SPUse > 0 && Sta.SP >= AtkD.SPUse);
        }

        // 変更を反映するための画像を表示
        AtkUIs[index].ChengedImages[!PAtk.Backs ? 0 : 1].SetActive(true);
    }

    #endregion
    void LateUpdate()
    {
        BaseSet();
        MPBar.fillAmount = Sta.MP / Mathf.Max(1, Sta.FMMP);
        MPFill.color = Sta.LowMP ? Color.red : Color.white;
        for (int i = 0; i < AtkUIs.Length; i++)
        {
            Data_Atk AtkD = null;
            bool Input = false;
            int Slot = i;
            AtkFBTx.text = !PAtk.Backs ? "表" : "裏";
            binaryUIAnimationo.UpdateAnimation(PAtk.Backs);

            var Atks = PriSetGet.AtkGet(PAtk.Backs);
            switch (i)
            {
                case 0:
                    AtkD = DB.N_Atks[Atks.N_AtkID];
                    Input = PCont.NAtk_Stay;
                    break;
                case 1:
                    AtkD = DB.S_Atks[Atks.S1_AtkID];
                    Input = PCont.S1Atk_Stay;
                    break;
                case 2:
                    AtkD = DB.S_Atks[Atks.S2_AtkID];
                    Input = PCont.S2Atk_Stay;
                    break;
                case 3:
                    AtkD = DB.E_Atks[Atks.E_AtkID];
                    Input = PCont.EAtk_Stay;
                    Slot = 10;
                    break;
            }

            foreach (var animation in AtkUIs[i].imageAnimation)
            {
                Debug.Log("animation");
                // 入力があった場合にアニメーションをPressedに変更
                animation.ChangeStatu(Input ? UISystem_Gabu.AnimatorStatu.Pressed : UISystem_Gabu.AnimatorStatu.Normal);
            }


            Sta.AtkCTs.TryGetValue(Slot, out var AtkCTs);
            UpdateStatus(i, AtkD, AtkCTs);
        }
        StateInfoTx.text = "MHP:" + Sta.FMHP;
        StateInfoTx.text += "\nMMP:" + Sta.FMMP;
        StateInfoTx.text += "\nAtk:" + Sta.FAtk;
        StateInfoTx.text += "\nDef:" + Sta.FDef;
        DebugUI.SetActive(SceneManager.GetActiveScene().buildIndex == 1);
    }
}
