using UnityEngine;
using System.Collections.Generic;

public class AsDlgBalloonMsgBoxQuest : AsDlgBalloonMsgBox
{
    public SpriteText spriteTextUnder;

    private int questCount = 0;

    protected List<SimpleSprite> listSimpleSprite = new List<SimpleSprite>();

    public void Init()
    {
        listSimpleSprite.Clear();

        listSimpleSprite.Add(left_top);
        listSimpleSprite.Add(top);
        listSimpleSprite.Add(right_top);
        listSimpleSprite.Add(left);
        listSimpleSprite.Add(center);
        listSimpleSprite.Add(right);
        listSimpleSprite.Add(left_bottom);
        listSimpleSprite.Add(bottom_left);
        listSimpleSprite.Add(tail);
        listSimpleSprite.Add(bottom_right);
        listSimpleSprite.Add(right_bottom);
    }

    public void CloseMsgBox()
    {
        GameObject.Destroy(gameObject);
    }

    public void UpdateText()
    {
        // find firstQuest
        List<ArkQuest> listSortedQuest = ArkQuestmanager.instance.GetSortedQuestList();

        ArkSphereQuestTool.QuestData questData = null;

        questCount = 0;

        foreach (ArkQuest quest in listSortedQuest)
        {
            ArkSphereQuestTool.QuestData nowQuestData = quest.GetQuestData();

            if (nowQuestData != null)
            {
                if (nowQuestData.NowQuestProgressState == QuestProgressState.QUEST_PROGRESS_IN)
                {
                    if (questData == null)
                        questData = quest.GetQuestData();

                    questCount++;
                }
            }
        }

        if (questData == null)
            return;

        AsLanguageManager.Instance.SetFontFromSystemLanguage(spriteText);
        AsLanguageManager.Instance.SetFontFromSystemLanguage(spriteTextUnder);


        spriteText.Color = new Color(1.0f, 0.9f, 0.094f);
        Tbl_QuestStringRecord questRecord = AsTableManager.Instance.GetTbl_QuestString_Record(questData.Info.ID);

        if (questRecord != null)
            spriteText.Text = questRecord.Info.Title;

        // set max width
        if (spriteText != null && spriteTextUnder != null)
        {
            spriteTextUnder.multiline = false;
            spriteText.multiline = false;
            spriteText.SetMaxWidth(spriteTextUnder.maxWidth);
        }

        // set under Text
        if (spriteTextUnder != null)
        {
            spriteTextUnder.Color = Color.white;
            if (questCount >= 2)
                spriteTextUnder.Text = string.Format(AsTableManager.Instance.GetTbl_String(4400), questCount - 1);
            else
                spriteTextUnder.Text = AsTableManager.Instance.GetTbl_String(4401);
        }

        UpdateShape();

        Visible = true;
    }


    public override void UpdateShape()
    {
        Vector3 centerPt = new Vector3(spriteText.transform.position.x, spriteText.transform.position.y + spriteText.lineSpacing * 0.5f, 0.0f);

        spriteTextUnder.transform.localPosition = new Vector3(spriteText.transform.localPosition.x, -(spriteText.lineSpacing * 0.5f + spriteTextUnder.lineSpacing * 0.5f), spriteText.transform.localPosition.z);

        // center
        center.transform.position = centerPt;

        if (questCount >= 2)
            center.width = (MINIMAL_WIDTH > spriteTextUnder.TotalWidth) ? MINIMAL_WIDTH : spriteTextUnder.TotalWidth;
        else
        {
            float width = spriteText.TotalWidth > spriteTextUnder.TotalWidth ? spriteText.TotalWidth : spriteTextUnder.TotalWidth;

            center.width = (MINIMAL_WIDTH > width) ? MINIMAL_WIDTH : width;
        }

        center.height = spriteText.lineSpacing + spriteTextUnder.lineSpacing;

        // left
        left.transform.position = new Vector3(centerPt.x - (center.width * 0.5f), centerPt.y, centerPt.z);
        left.height = center.height;
        left_bottom.transform.position = new Vector3(centerPt.x - (center.width * 0.5f), centerPt.y - (center.height * 0.5f), centerPt.z);

        // right
        right.transform.position = new Vector3(centerPt.x + (center.width * 0.5f), centerPt.y, centerPt.z);
        right.height = center.height;
        right_bottom.transform.position = new Vector3(centerPt.x + (center.width * 0.5f), centerPt.y - (center.height * 0.5f), centerPt.z);

        // top
        left_top.transform.position = new Vector3(centerPt.x - (center.width * 0.5f), centerPt.y + (center.height * 0.5f), centerPt.z);
        top.transform.position = new Vector3(centerPt.x, centerPt.y + (center.height * 0.5f), centerPt.z);
        top.width = center.width;
        right_top.transform.position = new Vector3(centerPt.x + (center.width * 0.5f), centerPt.y + (center.height * 0.5f), centerPt.z);

        // tail left/right size
        bottom_left.width = bottom_right.width = (center.width - tail.width) * 0.5f;

        // bottom
        switch (tailType)
        {
            case TailType.CENTER:
                tail.transform.position = new Vector3(centerPt.x, centerPt.y - (center.height * 0.5f), centerPt.z);
                bottom_left.transform.position = new Vector3(tail.transform.position.x - (tail.width * 0.5f) - (bottom_left.width * 0.5f), centerPt.y - (center.height * 0.5f), centerPt.z);
                bottom_right.transform.position = new Vector3(tail.transform.position.x + (tail.width * 0.5f) + (bottom_right.width * 0.5f), centerPt.y - (center.height * 0.5f), centerPt.z);
                break;
            case TailType.LEFT:
                tail.transform.position = new Vector3(left_bottom.transform.position.x + tail.width * 0.5f, centerPt.y - (center.height * 0.5f), centerPt.z);
                bottom_left.transform.position = new Vector3(tail.transform.position.x + (bottom_left.width * 0.5f) + tail.width * 0.5f, centerPt.y - (center.height * 0.5f), centerPt.z);
                bottom_right.transform.position = new Vector3(bottom_left.transform.position.x + (bottom_right.width), centerPt.y - (center.height * 0.5f), centerPt.z);
                break;
            case TailType.RIGHT:
                tail.transform.position = new Vector3(right_bottom.transform.position.x - tail.width * 0.5f, centerPt.y - (center.height * 0.5f), centerPt.z);
                bottom_left.transform.position = new Vector3(tail.transform.position.x - (bottom_left.width * 0.5f) - tail.width * 0.5f, centerPt.y - (center.height * 0.5f), centerPt.z);
                bottom_right.transform.position = new Vector3(bottom_left.transform.position.x - (bottom_right.width), centerPt.y - (center.height * 0.5f), centerPt.z);
                break;
        }

        top.CalcSize();
        left.CalcSize();
        center.CalcSize();
        right.CalcSize();
        bottom_left.CalcSize();
        bottom_right.CalcSize();

        if (frame != null)
            frame.transform.rotation = Quaternion.Euler(new Vector3(0.0f, 0.0f, frameRotation));

        Vector3 vOffset = transform.position - tail.transform.position;

        transform.position += new Vector3(vOffset.x, vOffset.y, 0.0f);

        // z reset
        foreach (SimpleSprite sprite in listSimpleSprite)
            sprite.transform.localPosition = new Vector3(sprite.transform.localPosition.x, sprite.transform.localPosition.y, 0.0f);
    }
}
