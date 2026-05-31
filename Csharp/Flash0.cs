using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Flash0 : MonoBehaviour
{
    public static Flash0 Instance { get; set; }
    private Material slashMaterial;
    private Renderer meshRenderer;
    private string Fill_Amount = "_Fill_Amount";

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private void EnsureInit()
    {
        if (meshRenderer == null) meshRenderer = GetComponent<Renderer>();
        // 使用 .material 會實例化材質
        if (slashMaterial == null && meshRenderer != null) slashMaterial = meshRenderer.material;
    }

    void Awake()
    {
        Instance = this;
        EnsureInit();
        meshRenderer.enabled = false;
    }
    public void PlayFlash()
    {
        EnsureInit();

        meshRenderer.enabled = true;
        slashMaterial.DOKill();
        slashMaterial.SetFloat(Fill_Amount, 1f);
        slashMaterial.DOFloat(0f, Fill_Amount, 0.3f)
            .SetEase(Ease.Linear);
        StartCoroutine(MainAttackRoutine());

    }
    IEnumerator MainAttackRoutine()
    {

        yield return new WaitForSeconds(0.3f);
        meshRenderer.enabled = false;
    }
}

