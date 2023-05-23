using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using AfricasTalkingCS;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using static Org.BouncyCastle.Math.EC.ECCurve;
using Microsoft.Extensions.Configuration.Xml;

namespace budget_tracker
{
    public class AppSetting
    {
        public List<string> AdminContacts { get; set; }
    }
}