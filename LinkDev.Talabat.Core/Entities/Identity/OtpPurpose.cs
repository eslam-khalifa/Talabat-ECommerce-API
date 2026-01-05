using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace LinkDev.Talabat.Core.Entities.Identity
{
    public enum OtpPurpose
    {
        [EnumMember(Value = "EmailConfirmation")]
        EmailConfirmation,
        [EnumMember(Value = "PasswordReset")]
        PasswordReset
    }
}
