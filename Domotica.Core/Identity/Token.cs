using shortid;
using shortid.Configuration;

namespace Domotica.Core.Identity;

public class Token
{
    public int Seed { get; } = 19641108;    // take birthday as seed 

    public Token(int? seed = null)
    {
        ShortId.SetSeed(seed ?? Seed);
    }

    public string Generate()
    {
        return ShortId.Generate(new GenerationOptions
        {
            Length = 12,
            UseNumbers = true,
            UseSpecialCharacters = false
        });
    }
}