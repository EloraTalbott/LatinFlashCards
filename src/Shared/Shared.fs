namespace Shared

type LatinText =
  | Normal of string
  | Macron of char

[<CLIMutable>]
type FlashCardFront = {Nominative:LatinText list; Genitive:LatinText list; }
type Gender = | Feminine | Masculine | Neuter
type Declension = | First | Second | Third | Fourth | Fifth

[<CLIMutable>]
type FlashCardBack = {Declension:Declension; EnglishTranslation:string; Lesson:int; EnglishDerivatives:string list; Gender:Gender; }

[<CLIMutable>]
type FlashCardData = {Front:FlashCardFront; Back:FlashCardBack; }