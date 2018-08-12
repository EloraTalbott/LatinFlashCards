namespace Shared

[<CLIMutable>]
type FlashCardFront = {Nominative:string; Genitive:string; }
type Gender = | Feminine | Masculine | Neuter
type Declension = | First | Second | Third | Fourth | Fifth

[<CLIMutable>]
type FlashCardBack = {Declension:Declension; EnglishTranslation:string; Lesson:int; EnglishDerivatives:string list; Gender:Gender; }

[<CLIMutable>]
type FlashCardData = {Front:FlashCardFront; Back:FlashCardBack; }