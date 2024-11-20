using Asp.Services.Attributes;

namespace Asp.Services.ConstantsServices;

using Services = ServiceAttribute;

[Services(Lifetime = ServiceLifetime.Singleton)]
public partial class Constants { }
