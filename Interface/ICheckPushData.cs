using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GoldCruise_DataPush.Models;

namespace GoldCruise_DataPush
{
    public interface ICheckPushData
    {
        bool IsPushed(pushdatainfo pushinfo);
    }
}
