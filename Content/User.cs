using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace KickDumbGiveawayBot.Content;

public class User
{
    public int dbId = -1;

    /// <summary>
    /// KickId
    /// </summary>
    public readonly long id;
    public readonly string name;
    public readonly bool isSpecial;

    public int spamCount;

    public User(long id, string name, bool isSpecial)
    {
        this.id = id;
        this.name = name;
        this.isSpecial = isSpecial;
    }
}
