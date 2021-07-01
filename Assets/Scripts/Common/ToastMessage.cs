using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ToastMessage : SingletonMonoBehavior<ToastMessage>
{
    public override int SortOrder => 1;

    [SerializeField]
    Text text;

    [SerializeField]
    CanvasGroup canvasGroup;

    public void ShowToast(string text, int duration = 2)
    {
        base.Show(false); // 창이 닫힐때 강제로 UI를 보이게 하면 무한 루프 발생한다.

        if (CacheGameObject.activeInHierarchy == false)
        {
            Debug.Log($"{text} 메시지는 표시안함, 토스트 UI를 활성화 시키지 못했음, 게임 끌때 발생함(정상 과정)");
            return;
        }

        // 기존에 활성화 중인 메시지가 있다면 지금 코루틴이 끝난 다음에 연속해서 보여주자.
        if (handle != null)
            StopCoroutine(handle);
        handle = StartCoroutine(showToastCo(text, duration));
    }

    Coroutine handle;

    private IEnumerator showToastCo(string text,
        int duration)
    {
        { //UI위치 안 맞아서 강제로 0위치로 이동시킴.
            RectTransform rt = GetComponent<RectTransform>();
            rt.offsetMin = Vector2.zero;
            rt.offsetMax = Vector2.zero;
        }

        Color orginalColor = this.text.color;

        this.text.text = text;
        this.text.enabled = true;

        //Fade in
        yield return fadeInAndOut(canvasGroup, true, 0.5f);

        //Wait for the duration
        float counter = 0;
        while (counter < duration)
        {
            counter += Time.deltaTime;
            yield return null;
        }

        //Fade out
        yield return fadeInAndOut(canvasGroup, false, 0.5f);

        this.text.enabled = false;
        this.text.color = orginalColor;

        Close();
    }

    IEnumerator fadeInAndOut(CanvasGroup canvasGroup, bool fadeIn, float duration)
    {
        float a, b;
        if (fadeIn)
        {
            a = 0f;
            b = 1f;
        }
        else
        {
            a = 1f;
            b = 0f;
        }

        float counter = 0f;

        while (counter < duration)
        {
            counter += Time.deltaTime;
            float alpha = Mathf.Lerp(a, b, counter / duration);

            canvasGroup.alpha = alpha;
            yield return null;
        }
    }
}
