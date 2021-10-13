using System;
using System.Collections.Generic;

namespace PhilipDaubmeier.WeConnectClient.Model.Capabilities
{
    public class ServiceInfo
    {
        public string ServiceId { get; set; } = string.Empty;
        public string ServiceType { get; set; } = string.Empty;
        public ServiceStatus ServiceStatus { get; set; } = new();
        public bool LicenseRequired { get; set; } = false;
        public CumulatedLicense CumulatedLicense { get; set; } = new();
        public bool PrimaryUserRequired { get; set; } = false;
        public bool TermsAndConditionsRequired { get; set; } = false;
        public bool BlockDisabling { get; set; } = false;
        public bool Personalized { get; set; } = false;
        public bool Configurable { get; set; } = false;
        public bool RolesAndRightsRequired { get; set; } = false;
        public string DeviceType { get; set; } = string.Empty;
        public DateTime ServiceEol { get; set; } = DateTime.MinValue;
        public PrivacyGroupList PrivacyGroups { get; set; } = new();
        public Parameters Parameters { get; set; } = new();
        public UrlContent InvocationUrl { get; set; } = new();
        public List<Operation> Operation { get; set; } = new();
    }

    public class Operation
    {
        public string Id { get; set; } = string.Empty;
        public int Version { get; set; } = 0;
        public string Xsd { get; set; } = string.Empty;
        public string Permission { get; set; } = string.Empty;
        public string RequiredUserRole { get; set; } = string.Empty;
        public string RequiredSecurityLevel { get; set; } = string.Empty;
        public UrlContent InvocationUrl { get; set; } = new();
    }

    public class ServiceStatus
    {
        public string Status { get; set; } = string.Empty;
    }

    public class PrivacyGroupList
    {
        public List<string> PrivacyGroup { get; set; } = new();
    }

    public class Parameters
    {
        public List<Parameter> Parameter { get; set; } = new();
    }

    public class Parameter
    {
        public string Content { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
    }

    public class UrlContent
    {
        public string Content { get; set; } = string.Empty;
    }
}