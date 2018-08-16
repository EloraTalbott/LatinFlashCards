module FlashCards

open Shared
open FSharp.Data

let allFlashCards = 
//There are 15 flash cards
//I dont know the declensions of some of the cards
//How do you put macrons?
//Yes we need to put them in
//Both of my teachers say I need to know where the macrons go
//They are the lines above some of the vowels
//I closed everything to make it easier to navigate the flash cards
  [
    {
//1
        Front = {Nominative = "nauta"; Genitive = "nautae" }
        Back = 
          {
            Declension = Declension.First
            Lesson = 1
            EnglishTranslation = "sailor"
            EnglishDerivatives = ["nautical"; "nautilus";]
            Gender = Gender.Masculine
          }
    }
    {
//2
        Front = {Nominative = "terra"; Genitive = "terrae" }
        Back = 
          {
            Declension = Declension.First
            Lesson = 1
            EnglishTranslation = "earth, land"
            EnglishDerivatives = ["territory, terrestrial, terrain";]
            Gender = Gender.Feminine
          }
    }
    {
//3
        Front = {Nominative = "porta"; Genitive = "portae" }
        Back = 
          {
            Declension = Declension.First
            Lesson = 1
            EnglishTranslation = "gate"
            EnglishDerivatives = ["portal, airport, portico";]
            Gender = Gender.Feminine
          }
    }  
    {
//4
        Front = {Nominative = "silva"; Genitive = "silvae" }
        Back = 
          {
            Declension = Declension.First
            Lesson = 1
            EnglishTranslation = "forest"
            EnglishDerivatives = ["silviculture";]
            Gender = Gender.Feminine
          }
    }
    {
//5
        Front = {Nominative = "gladius"; Genitive = "gladii" }
        Back = 
          {
            Declension = Declension.Second
            Lesson = 0
            EnglishTranslation = "sword"
            EnglishDerivatives = ["How am I supposed to know";]
            Gender = Gender.Masculine
          }
    }
    {
//6
        Front = {Nominative = "servus"; Genitive = "servi" }
        Back = 
          {
            Declension = Declension.Second
            Lesson = 0
            EnglishTranslation = "slave"
            EnglishDerivatives = ["How am I suposed to know";]
            Gender = Gender.Masculine
          }
    }
    {
//7
        Front = {Nominative = "caelum"; Genitive = "caeli" }
        Back = 
          {
            Declension = Declension.Second
            Lesson = 0
            EnglishTranslation = "sky,heaven"
            EnglishDerivatives = ["How am I supposed to know";]
            Gender = Gender.Neuter
          }
    }
    {
//8
        Front = {Nominative = "fillius"; Genitive = "filii" }
        Back = 
          {
            Declension = Declension.First
            Lesson = 0
            EnglishTranslation = "son"
            EnglishDerivatives = ["How am I supposed to know";]
            Gender = Gender.Masculine
          }
    }
    {
//9
        Front = {Nominative = "amicus"; Genitive = "amici" }
        Back = 
          {
            Declension = Declension.Second
            Lesson = 0
            EnglishTranslation = "friend"
            EnglishDerivatives = ["How am I supposed to know";]
            Gender = Gender.Masculine
          }
    }
    {
//10
        Front = {Nominative = "romanus"; Genitive = "romani" }
        Back = 
          {
            Declension = Declension.Second
            Lesson = 0
            EnglishTranslation = "roman"
            EnglishDerivatives = ["How am I supposed to know";]
            Gender = Gender.Masculine
          }
    }
    {
//11
        Front = {Nominative = "dux"; Genitive = "ducis" }
        Back = 
          {
            Declension = Declension.First
            Lesson = 0
            EnglishTranslation = "leader"
            EnglishDerivatives = ["How am I supposed to know";]
            Gender = Gender.Masculine
          }
    }
    {
//12
//need macrons
        Front = {Nominative = "Maria"; Genitive = "Mariae" }
        Back = 
          {
            Declension = Declension.First
            Lesson = 1
            EnglishTranslation = "Mary"
            EnglishDerivatives = ["How am I supposed to know";]
            Gender = Gender.Feminine
          }
    }
    {
//13
//need macrons
        Front = {Nominative = "gloria"; Genitive = "gloriae" }
        Back = 
          {
            Declension = Declension.First
            Lesson = 1
            EnglishTranslation = "fame, glory"
            EnglishDerivatives = ["glorious, glory";]
            Gender = Gender.Feminine
          }
//15
//Empty flash cards
    }
    {
        Front = {Nominative = ""; Genitive = "" }
        Back = 
          {
            Declension = Declension.First
            Lesson = 0
            EnglishTranslation = ""
            EnglishDerivatives = ["How am I supposed to know";]
            Gender = Gender.Feminine
          }
    }   
    {
        Front = {Nominative = ""; Genitive = "" }
        Back = 
          {
            Declension = Declension.First
            Lesson = 0
            EnglishTranslation = ""
            EnglishDerivatives = ["How am I supposed to know";]
            Gender = Gender.Feminine
          }
    }
    {
        Front = {Nominative = ""; Genitive = "" }
        Back = 
          {
            Declension = Declension.First
            Lesson = 0
            EnglishTranslation = ""
            EnglishDerivatives = ["How am I supposed to know";]
            Gender = Gender.Feminine
          }
    }
  ]

let getRandom () =  
  let rnd = System.Random()  
  allFlashCards |> List.item (rnd.Next(allFlashCards.Length)) 