/**
* Author: Christopher Cola
* Created on 20/04/2016
*/

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Mobo
{
    // Extension class to ignore the removed value output when using ConcurrentDictionary.TryRemove
    // Necessary because otherwise you have to assign the removed thing to a new variable which is annoying
    public static class DictionaryRemove
    {
        public static bool Remove<TKey, TValue>(
          this ConcurrentDictionary<TKey, TValue> self, TKey key)
        {
            TValue nope;
            return self.TryRemove(key, out nope);
        }
    }
}
