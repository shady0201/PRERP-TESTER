using CommunityToolkit.Mvvm.Messaging.Messages;
using PRERP_TESTER.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PRERP_TESTER.Messages
{
    public class ShowAccountDetailMessage(Account account) : ValueChangedMessage<Account>(account) { }
}
