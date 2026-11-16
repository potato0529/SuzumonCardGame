using UnityEngine;

public class GuideLink : MonoBehaviour,IHyperLink
{
    public MainGuideSeries mainGuideSeries;

    public void EnterLink(string link)
    {
        
    }

    public void ExitLink(string link)
    {

    }

    public void ClickLink(string link)
    {
        mainGuideSeries.ShowGuide();

        switch (link)
        {
            case "monster":
                mainGuideSeries.ShowSlide(6);
                break;

            case "item":
                mainGuideSeries.ShowSlide(7);
                break;

            case "dad":
                mainGuideSeries.ShowSlide(0);
                break;

        }
    }
}
