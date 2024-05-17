using UnityEngine.UI;

public class BeatIndicator : BeatListener
{
    Slider indicator;
    private void Start()
    {
        indicator = GetComponent<Slider>();
        animatedValueStart = 0;
        animatedValueEnd = 1;
        AudioManager.instance.AddListener(this);
    }

    private void Update()
    {
        indicator.value = animatedValue;
    }
}
