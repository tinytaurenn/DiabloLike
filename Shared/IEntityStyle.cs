using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Shared
{
    public interface IEntityStyle
    {
        
         enum Style
        {
            Good,
            Evil
        }

         Style GetStyle(); 


         //void SetStyle(Style style);  



    }
}
