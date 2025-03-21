using TMPro;
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.SDK3.StringLoading;
using VRC.Udon;
using VRC.Udon.Common.Interfaces;

namespace Llamahat
{
    [UdonBehaviourSyncMode(BehaviourSyncMode.None)]
    public class SimpleStringLoader : UdonSharpBehaviour
    {
        [Header("Download Link")]
        public VRCUrl url;

        [Header("Text Components")]
        [Tooltip("Text Mesh Pro UGUI component the string should be applied to")]
        [SerializeField] private TextMeshProUGUI TestMeshPro;

        public Color Tier1Color = new Color(5f / 255f, 179f / 255f, 13f / 255f); // Dark Green
        public Color Tier2Color = new Color(255f / 255f, 43f / 255f, 0f / 255f); // Dark Green
        [Header("Tier 3 Color is set via ColorGradint in TestMeshPro")]


        [Header("Loading & Error String")]
        [Tooltip("Setting refreshTime to 0 make its load only once")]
        [SerializeField] private float RefreshPeriod = 180;
        [SerializeField] private string loadingString = "Loading...";
        [Tooltip("Use the Error String when the String couldn't be Loaded")]
        [SerializeField] private bool useErrorString = true;
        [Tooltip("String used when the String couldn't be Loaded")] [TextArea]
        [SerializeField] private string errorString = "Error: String couldn't be loaded, view logs for more info";






        private void Start()
        {
            VRCStringDownloader.LoadUrl(url, (IUdonEventReceiver)this);
        }

        public void _LoadString()
        {
            VRCStringDownloader.LoadUrl(url, (IUdonEventReceiver)this);
        }
        public override void OnStringLoadSuccess(IVRCStringDownload result)
        {
            ApplyPatreons(result.Result);
        }
        public override void OnStringLoadError(IVRCStringDownload result)
        {
            //Loading faled so dislay error message :/
            if (TestMeshPro != null) TestMeshPro.text = "<color=#FFF>" + errorString + "</color>";
        }

        private void ApplyPatreons(string jsonString)
        {

            string tier1Names = ExtractNames(jsonString, "Supporter-Tier1");
            string tier2Names = ExtractNames(jsonString, "Supporter-Tier2");
            string tier3Names = ExtractNames(jsonString, "Supporter-Tier3");

            if (TestMeshPro != null) TestMeshPro.text = CombinePatreons(tier1Names, tier2Names, tier3Names);
        }


        public void RefreshPatreonList()
        {
            if(RefreshPeriod > 0)
            {
                SendCustomEventDelayedSeconds("_LoadString", RefreshPeriod);
            }

        }




        private string CombinePatreons(string tier1Names, string tier2Names, string tier3Names)
        {

            //Takes the List Of Names and Combines and formats them in a Colored string^^
            string formattedText = "";

            if (!string.IsNullOrEmpty(tier1Names))
            {
                formattedText += "<color=" + ColorToHex(Tier1Color) + ">" + tier1Names + "</color> \n\n";
            }

            if (!string.IsNullOrEmpty(tier2Names))
            {
                formattedText += "<color=" + ColorToHex(Tier2Color) + ">" + tier2Names + "</color> \n\n";
            }

            if (!string.IsNullOrEmpty(tier3Names))
            {
                formattedText += "<b>" + tier3Names + "</b>\n";
            }

            return formattedText;
        }

        private string ColorToHex(Color color)
        {
            //Convert RGB color from input to Hex values
            int r = Mathf.Clamp((int)(color.r * 255), 0, 255);
            int g = Mathf.Clamp((int)(color.g * 255), 0, 255);
            int b = Mathf.Clamp((int)(color.b * 255), 0, 255);
            return string.Format("#{0:X2}{1:X2}{2:X2}", r, g, b);
        }



        private string ExtractNames(string json, string key)
        {
            
            int startIndex = json.IndexOf("\"" + key + "\"");
            if (startIndex == -1) return "None";

            startIndex = json.IndexOf("[", startIndex);
            int endIndex = json.IndexOf("]", startIndex);
            if (startIndex == -1 || endIndex == -1) return "None";

            string namesSection = json.Substring(startIndex + 1, endIndex - startIndex - 1);

            //Remove Whitespaces and Format Properly^^
            //Removes <> so formating cant be broken exidently
            namesSection = namesSection.Replace("\"", "").Replace("\n", "").Replace("\t", "").Replace("<", "").Replace(">", "");


            string[] names = namesSection.Split(',');
            for (int i = 0; i < names.Length; i++)
            {
                names[i] = names[i].Trim();
            }

            return names.Length > 0 && !string.IsNullOrWhiteSpace(names[0]) ? string.Join(", ", names) : "";
        }

    }
}

