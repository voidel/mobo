/**
* Author: Christopher Cola
* Created on 20/04/2016
*/

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MoboServerWPF
{
    // Extension class to ignore the removed value output when using ConcurrentDictionary.TryRemove
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
