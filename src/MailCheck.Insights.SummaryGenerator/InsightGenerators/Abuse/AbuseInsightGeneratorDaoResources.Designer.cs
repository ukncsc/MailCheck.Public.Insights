﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace MailCheck.Insights.SummaryGenerator.InsightGenerators.Abuse {
    using System;
    
    
    /// <summary>
    ///   A strongly-typed resource class, for looking up localized strings, etc.
    /// </summary>
    // This class was auto-generated by the StronglyTypedResourceBuilder
    // class via a tool like ResGen or Visual Studio.
    // To add or remove a member, edit your .ResX file then rerun ResGen
    // with the /str option, or rebuild your VS project.
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("System.Resources.Tools.StronglyTypedResourceBuilder", "16.0.0.0")]
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    public class AbuseInsightGeneratorDaoResources {
        
        private static global::System.Resources.ResourceManager resourceMan;
        
        private static global::System.Globalization.CultureInfo resourceCulture;
        
        [global::System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal AbuseInsightGeneratorDaoResources() {
        }
        
        /// <summary>
        ///   Returns the cached ResourceManager instance used by this class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        public static global::System.Resources.ResourceManager ResourceManager {
            get {
                if (object.ReferenceEquals(resourceMan, null)) {
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("MailCheck.Insights.SummaryGenerator.InsightGenerators.Abuse.AbuseInsightGenerator" +
                            "DaoResources", typeof(AbuseInsightGeneratorDaoResources).Assembly);
                    resourceMan = temp;
                }
                return resourceMan;
            }
        }
        
        /// <summary>
        ///   Overrides the current thread's CurrentUICulture property for all
        ///   resource lookups using this strongly typed resource class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        public static global::System.Globalization.CultureInfo Culture {
            get {
                return resourceCulture;
            }
            set {
                resourceCulture = value;
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to 
        ///    SELECT 
        ///      count Count, 
        ///      host_name HostName,
        ///      host_provider HostProvider,
        ///      bounce_reflector_blocklist_count BounceReflectorBlocklistCount, 
        ///      end_user_blocklist_count EndUserBlocklistCount, 
        ///      end_user_network_blocklist_count EndUserNetworkBlocklistCount, 
        ///      hijacked_network_blocklist_count HijackedNetworkBlocklistCount, 
        ///      malware_blocklist_count MalwareBlockListCount, 
        ///      proxy_blocklist_count ProxyBlocklistCount,
        ///      spam_source_blocklist_count Spam [rest of string was truncated]&quot;;.
        /// </summary>
        public static string GetRawData {
            get {
                return ResourceManager.GetString("GetRawData", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to SELECT 
        ///      count Count, 
        ///      host_name HostName,
        ///      host_provider HostProvider,
        ///      bounce_reflector_blocklist_count BounceReflectorBlocklistCount, 
        ///      end_user_blocklist_count EndUserBlocklistCount, 
        ///      end_user_network_blocklist_count EndUserNetworkBlocklistCount, 
        ///      hijacked_network_blocklist_count HijackedNetworkBlocklistCount, 
        ///      malware_blocklist_count MalwareBlockListCount, 
        ///      proxy_blocklist_count ProxyBlocklistCount,
        ///      spam_source_blocklist_count SpamSource [rest of string was truncated]&quot;;.
        /// </summary>
        public static string GetRawSubdomainData {
            get {
                return ResourceManager.GetString("GetRawSubdomainData", resourceCulture);
            }
        }
    }
}
