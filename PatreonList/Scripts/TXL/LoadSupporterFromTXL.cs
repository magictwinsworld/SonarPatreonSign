using System;
using TMPro;
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.SDK3.StringLoading;
using VRC.Udon;
using VRC.Udon.Common.Interfaces;

namespace magictwin
{
    [UdonBehaviourSyncMode(BehaviourSyncMode.None)]
    public class LoadSupporterFromTXL : UdonSharpBehaviour
    {
        [Header("TXL Remote Lists")]
        public Texel.AccessControlRemoteUserList ACRUL_Tier1;
        public Texel.AccessControlRemoteUserList ACRUL_Tier2;
        public Texel.AccessControlRemoteUserList ACRUL_Tier3;

        [Header("Text Components")]
        [Tooltip("Text Mesh Pro UGUI component the string should be applied to")]
        [SerializeField] private TextMeshProUGUI TestMeshPro;

        public Color Tier1Color = new Color(5f / 255f, 179f / 255f, 13f / 255f); // Dark Green
        public Color Tier2Color = new Color(255f / 255f, 43f / 255f, 0f / 255f); // Dark Green
        [Header("Tier 3 Color is set via ColorGradint in TestMeshPro")]


        [Header("Loading & Error String")]
        [Tooltip("Setting refreshTime to 0 make its load only once")]
        [SerializeField] private float RefreshPeriod = 180;
        [SerializeField] private float StartDelay = 5;
        [SerializeField] private string loadingString = "Loading...";
        [Tooltip("Use the Error String when the String couldn't be Loaded")]
        [SerializeField] private bool useErrorString = true;
        [Tooltip("String used when the String couldn't be Loaded")]
        [TextArea]
        [SerializeField] private string errorString = "Error: String couldn't be loaded, view logs for more info";


        private void Start()
        {
            if (TestMeshPro != null) TestMeshPro.text = "<color=\"white\">" + loadingString +"</color>";

            SendCustomEventDelayedSeconds("ApplyPatreons", StartDelay);
        }



        public void RefreshPatreonList()
        {
            if(RefreshPeriod > 0)
            {
                SendCustomEventDelayedSeconds("ApplyPatreons", RefreshPeriod);
            }
        }




        public void ApplyPatreons()
        {
            string tier1Names = "";
            string tier2Names = "";
            string tier3Names = "";

            if (ACRUL_Tier1.UserList != null || ACRUL_Tier1.UserList.Length != 0)
            {
                tier1Names = string.Join(", ", ACRUL_Tier1.UserList);
            }


            if (ACRUL_Tier2.UserList != null || ACRUL_Tier2.UserList.Length != 0)
            {
                tier2Names = string.Join(", ", ACRUL_Tier2.UserList);
            }

            if (ACRUL_Tier3.UserList != null || ACRUL_Tier3.UserList.Length != 0)
            {
                tier3Names = string.Join(", ", ACRUL_Tier3.UserList);
            }


            if (TestMeshPro != null) TestMeshPro.text = CombinePatreons(tier1Names, tier2Names, tier3Names);

            RefreshPatreonList();
        }



        public string CombinePatreons(string tier1Names, string tier2Names, string tier3Names)
        {

            //Takes the List Of Names and Combines and formats them in a Colored string^^
            string formattedText = "";

            if (!string.IsNullOrEmpty(tier1Names))
            {
                formattedText += "<color=" + ColorToHex(Tier1Color) + ">" + removeFormateBreaker(tier1Names) + "</color> \n\n";
            }

            if (!string.IsNullOrEmpty(tier2Names))
            {
                formattedText += "<color=" + ColorToHex(Tier2Color) + ">" + removeFormateBreaker(tier2Names) + "</color> \n\n";
            }

            if (!string.IsNullOrEmpty(tier3Names))
            {
                formattedText += "<b>" + removeFormateBreaker(tier3Names) + "</b>\n";
            }

            return formattedText;
        }

       private string removeFormateBreaker(string tierNames)
        {
            tierNames = tierNames.Replace("\"", "").Replace("\n", "").Replace("\t", "").Replace("<", "").Replace(">", "");
            return tierNames;
        }

        private string ColorToHex(Color color)
        {
            //Convert RGB color from input to Hex values
            int r = Mathf.Clamp((int)(color.r * 255), 0, 255);
            int g = Mathf.Clamp((int)(color.g * 255), 0, 255);
            int b = Mathf.Clamp((int)(color.b * 255), 0, 255);
            return string.Format("#{0:X2}{1:X2}{2:X2}", r, g, b);
        }

    }
}

