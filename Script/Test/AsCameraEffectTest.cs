using UnityEngine;
using System.Collections;

[ExecuteInEditMode]
public class AsCameraEffectTest : MonoBehaviour
{
   
    //---------------------------------------------------------------------
    /* Variable */
    //---------------------------------------------------------------------		
    public  Transform m_CharTransform = null;
    private Vector3 m_vec3LocalPosition = Vector3.zero;
    public float m_fTargetHeight = 0.0f;
    public float m_fYRotate = 0.0f;
	public float fDistance =0.0f;
	public float fHeight= 0.0f;


    public void SetLocalPosition(float fDistance, float fHeight, float fYRotate)
    {
        m_vec3LocalPosition = new Vector3(0.0f, fHeight, fDistance);
        m_fYRotate = fYRotate;

        m_vec3LocalPosition = Quaternion.AngleAxis(m_fYRotate, Vector3.up) * m_vec3LocalPosition;
    }

    public void SetLookTarget(Transform charTransform, float fTargetHeight)
    {
        m_CharTransform = charTransform;
        m_fTargetHeight = fTargetHeight;
    }

    public Vector3 GetCameraPos()
    {
        return transform.position;
    }

    public void UpdateNormal()
    {
        Vector3 position;
        if (null == m_CharTransform)
        {
            position = Vector3.zero;
        }
        else
        {
            position = m_CharTransform.position;
        }



        transform.position = position + m_vec3LocalPosition;

        // char look pos
        Vector3 CharLookPos = position;
        CharLookPos.y += m_fTargetHeight;

        // set rotate
        Vector3 relativePos = CharLookPos - transform.position;
        Quaternion rotation = Quaternion.LookRotation(relativePos);
        transform.rotation = rotation;
    }
    
    //---------------------------------------------------------------------
    /* Virtual */
    //---------------------------------------------------------------------	


    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        SetLocalPosition(fDistance, fHeight, m_fYRotate);
        SetLookTarget(m_CharTransform, m_fTargetHeight);

    }


    // Update is called once per frame
    void LateUpdate()
    {
        
        UpdateNormal();
    }
}
