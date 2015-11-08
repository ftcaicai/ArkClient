using UnityEngine;
using System.Collections;

public enum QuestProgressionPanelState
{
    NORMAL,
    APPEARING,
    SHOW,
    HIDING,
    HIDED,
}

public class QuestProgressMsgPanel : MonoBehaviour
{
    public QuestProgressionPanelState nowState = QuestProgressionPanelState.NORMAL;
    public AsDlgBase bgGride;
    public UIInteractivePanel interPanel;
    public SpriteText         spriteText;

    float   fShowTime = 0.0f;
    float   fTime = 0.0f;
    Vector3 originalPos;

    EZTransition bringInFoward;
    EZTransition dismissForwad;

    void Start()
    {
        bringInFoward = interPanel.GetTransition(UIPanelManager.SHOW_MODE.BringInForward);
        dismissForwad = interPanel.GetTransition(UIPanelManager.SHOW_MODE.DismissForward);

        bringInFoward.AddTransitionEndDelegate(AppearEnd);
        dismissForwad.AddTransitionEndDelegate(HideEnd);
    }

    public void Init(QuestProgressMsgInfo _info, float _fAppearTime, float _fShowTime, float _fHideTime)
    {
        dismissForwad.RemoveTransitionEndDelegate(HideEnd);
        dismissForwad.End();
        dismissForwad.AddTransitionEndDelegate(HideEnd);

        spriteText.Text         = _info.szMsg;
        spriteText.Color        = _info.color;


		bgGride.SetColor(Color.white);
        bgGride.width = spriteText.TotalWidth;
        bgGride.Assign();

        transform.localPosition = new Vector3(bgGride.TotalWidth * 0.5f, 0.0f, 0.0f);
       
        fShowTime = _fShowTime;

        bringInFoward.animParams[0].duration = _fAppearTime;
        bringInFoward.animParams[0].vec = new Vector3(-bgGride.TotalWidth, 0.0f, 0.0f);
        dismissForwad.animParams[0].duration = _fHideTime;
       // dismissForwad.animParams[1].duration = _fHideTime;


        nowState = QuestProgressionPanelState.NORMAL;
    }

    public void ResetShowTime(float _fShowTime, Vector3 vPos, float fHideTime, string _msg, Color _Color)
    {
        dismissForwad.RemoveTransitionEndDelegate(HideEnd);
        dismissForwad.StopSafe();

        fTime = 0.0f;
        fShowTime = _fShowTime;
        nowState = QuestProgressionPanelState.SHOW;
        spriteText.Color = new Color(_Color.r, _Color.g, _Color.b, 1.0f);
        spriteText.Text = _msg;

        transform.localPosition = vPos;

       // bgGride.transform.localPosition = new Vector3(bgGride.width * 0.5f, 0.0f, 1.0f);
		bgGride.SetColor(Color.white);
        bgGride.width = spriteText.TotalWidth;
        bgGride.Assign();

      
        dismissForwad.animParams[0].duration = fHideTime;
       // dismissForwad.animParams[1].duration = fHideTime;

        dismissForwad.AddTransitionEndDelegate(HideEnd);
    }

	void Update () 
    {
        if (nowState == QuestProgressionPanelState.SHOW)
        {
            if (fTime >= fShowTime)
            {
                fTime = 0.0f;

                dismissForwad.Start();
                nowState = QuestProgressionPanelState.HIDING;
            }
            else
                fTime += Time.deltaTime;

			
        }

		 if (nowState == QuestProgressionPanelState.HIDING)
			 bgGride.SetColor(new Color(1.0f, 1.0f, 1.0f, spriteText.Color.a));
	}

    public void ResetHiding()
    {
        if (nowState == QuestProgressionPanelState.HIDING)
            dismissForwad.End();
        else if (nowState == QuestProgressionPanelState.APPEARING)
            bringInFoward.End();

        nowState = QuestProgressionPanelState.HIDED;
		bgGride.SetColor(new Color(1.0f, 1.0f, 1.0f, 0.0f));
        spriteText.Color = new Color(1.0f, 1.0f, 1.0f, 0.0f);
    }

    public void Appear()
    {
        interPanel.Reveal();
        nowState = QuestProgressionPanelState.APPEARING;
    }

    public void Show()
    {
        nowState = QuestProgressionPanelState.SHOW;
        fTime = 0.0f;
    }

    void AppearEnd(EZTransition transition)
    {
        fTime    = 0.0f;
        nowState = QuestProgressionPanelState.SHOW;
    }

    void HideEnd(EZTransition transition)
    {
        nowState = QuestProgressionPanelState.HIDED;
    }

    //void OnGUI()
    //{
    //    if (GUI.Button(new Rect(0, 0, 100, 100), "init"))
    //        Init("Do you hear me?", transform.position, 0.5f, 0.8f, 0.8f); 

    //    if (GUI.Button(new Rect(0, 110, 100, 100), "시작"))
    //        Appear();

    //    if (GUI.Button(new Rect(0, 220, 100, 100), "리셋"))
    //        ResetShowTime(0.8f);
    //}
}
