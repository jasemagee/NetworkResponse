using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.NetworkInformation;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace NetworkResponse
{
    [Serializable]
    public class NetworkTarget
    {
        public string Name { get; set; }
        public string Target { get; set; }
        
        public DateTime Checked { get; set; }

        public long ResponseMs { get; set; }

        private List<string> mErrors;
        public string[] Errors { get { return mErrors.ToArray(); } }


        public void Update()
        {
            mErrors = new List<string>();

            if (string.IsNullOrEmpty(Target))
                return;
  
            var ping = new Ping();
            var options = new PingOptions() { DontFragment = true };
            
            // Create some random data to be transmit
            byte[] buffer = new byte[32];
            var rand = new Random();
            rand.NextBytes(buffer);

            int timeout = 120;
            try
            {
                var reply = ping.Send(Target, timeout, buffer, options);
                if (reply.Status == IPStatus.Success)
                {
                    Checked = DateTime.Now;
                    ResponseMs = reply.RoundtripTime;
                }
            }
            catch (PingException ex)
            {
                mErrors.Add(ex.Message);
            }
        }

        public static List<NetworkTarget> GetNetworkTargetsFromProperties()
        {
            byte[] bytes = Convert.FromBase64String(Properties.Settings.Default.Targets);
            if (bytes.Length == 0)
                return new List<NetworkTarget>();

            using (var ms = new MemoryStream(bytes, 0, bytes.Length))
            {
                ms.Write(bytes, 0, bytes.Length);
                ms.Position = 0;
                return (List<NetworkTarget>)(new BinaryFormatter().Deserialize(ms));
            }
        }

        public static void SaveNetworkTargetsToProperties(List<NetworkTarget> list)
        {
            using (var ms = new MemoryStream())
            {
                new BinaryFormatter().Serialize(ms, list);
                var listBase64 = Convert.ToBase64String(ms.ToArray());
                Properties.Settings.Default.Targets = listBase64;
                Properties.Settings.Default.Save();
            }
        }
    }
}
