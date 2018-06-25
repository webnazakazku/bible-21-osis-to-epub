using System.Collections.Generic;

namespace BibleDoEpubu.ObjektovyModel
{
  /// <summary>
  /// Top-level objekt reprezentující celou Bibli.
  /// </summary>
  internal class Bible
  {
    #region Vlastnosti

    public List<Kniha> Knihy
    {
      get;
      set;
    } = new List<Kniha>();

    public Dictionary<string, InformaceOKnize> MapovaniZkratekKnih
    {
      get;
    }

    public Metadata Metadata
    {
      get;
      set;
    }

    public Revize Revize
    {
      get;
      set;
    }

    #endregion

    #region Konstruktory

    public Bible()
    {
      MapovaniZkratekKnih = NacistMapovaniZkratekKnih();
    }

    #endregion

    #region Metody

    private Dictionary<string, InformaceOKnize> NacistMapovaniZkratekKnih()
    {
            return new Dictionary<string, InformaceOKnize>
      {
        // Starý zákon.
        {"Gen", "Gn;Genesis;genesis"},
        {"Exod", "Ex;Exodus;exodus"},
        {"Lev", "Lv;Leviticus;leviticus"},
        {"Num", "Nu;Numeri;numeri"},
        {"Deut", "Dt;Deuteronomium;deuteronomium"},
        {"Josh", "Joz;Jozue;jozue"},
        {"Judg", "Sd;Soudců;soudcu"},
        {"Ruth", "Rt;Rút;rut"},
        {"1Sam", "1S;1. Samuel;1samuel"},
        {"2Sam", "2S;2. Samuel;2samuel"},
        {"1Kgs", "1Kr;1. Královská;1kralovska"},
        {"2Kgs", "2Kr;2. Královská;2kralovska"},
        {"1Chr", "1Pa;1. Letopisů;1letopisu"},
        {"2Chr", "2Pa;2. Letopisů;2letopisu"},
        {"Ezra", "Ezd;Ezdráš;ezdras"},
        {"Neh", "Neh;Nehemiáš;nehemias"},
        {"Esth", "Est;Ester;ester"},
        {"Job", "Jb;Job;job"},
        {"Ps", "Ž;Žalmy;zalmy"},
        {"Prov", "Př;Přísloví;prislovi"},
        {"Eccl", "Kaz;Kazatel;kazatel"},
        {"Song", "Pís;Píseň písní;pisen"},
        {"Isa", "Iz;Izaiáš;izaias"},
        {"Jer", "Jr;Jeremiáš;jeremias"},
        {"Lam", "Pl;Pláč;plac"},
        {"Ezek", "Ez;Ezechiel;ezechiel"},
        {"Dan", "Dn;Daniel;daniel"},
        {"Hos", "Oz;Ozeáš;ozeas"},
        {"Joel", "Jl;Joel;joel"},
        {"Amos", "Am;Amos;amos"},
        {"Obad", "Abd;Abdiáš;abdias"},
        {"Jonah", "Jon;Jonáš;jonas"},
        {"Nah", "Na;Nahum;nahum"},
        {"Mic", "Mi;Micheáš;micheas"},
        {"Hab", "Ab;Abakuk;abakuk"},
        {"Zeph", "Sf;Sofoniáš;sofonias"},
        {"Hag", "Ag;Ageus;ageus"},
        {"Zech", "Za;Zachariáš;zacharias"},
        {"Mal", "Mal;Malachiáš;malachias"},

        // Deuterokanické knihy (ke Starému zákonu).
        {"Tob", "Tob;Tobiáš;tobias"},
        {"Bar", "Bar;Baruch;baruch"},
        {"AddEsth", "Estp;Ester;dtk_ester"},
        {"AddDan", "Danp;Přídavky k Danielovi;pridavky_k_danielovi"},
        {"Sir", "Sír;Sirachovec;sirachovec"},
        {"Jdt", "Jud;Judita;judita"},
        {"Wis", "Mdr;Moudrost Šalomounova;moudrost_salomounova"},
        {"1Macc", "1Mak;1. Makabejská;1makabejska"},
        {"2Macc", "2Mak;2. Makabejská;2makabejska"},

        // Nový zákon.
        {"Matt", "Mt;Matouš;matous"},
        {"Mark", "Mk;Marek;marek"},
        {"Luke", "L;Lukáš;lukas"},
        {"John", "J;Jan;jan"},
        {"Acts", "Sk;Skutky;skutky"},
        {"Rom", "Ř;Římanům;rimanum"},
        {"1Cor", "1Kor;1. Korintským;1korintskym"},
        {"2Cor", "2Kor;2. Korintským;2korintskym"},
        {"Gal", "Ga;Galatským;galatskym"},
        {"Eph", "Ef;Efeským;efeskym"},
        {"Phil", "Fp;Filipským;filipskym"},
        {"Col", "Ko;Koloským;koloskym"},
        {"1Thess", "1Te;1. Tesalonickým;1tesalonickym"},
        {"2Thess", "2Te;2. Tesalonickým;2tesalonickym"},
        {"1Tim", "1Tm;1. Timoteus;1timoteovi"},
        {"2Tim", "2Tm;2. Timoteus;2timoteovi"},
        {"Titus", "Tt;Titus;titus"},
        {"Phlm", "Fm;Filemon;filemon"},
        {"Heb", "Žd;Židům;zidum"},
        {"Jas", "Jk;Jakub;jakub"},
        {"1Pet", "1Pt;1. Petr;1petr"},
        {"2Pet", "2Pt;2. Petr;2petr"},
        {"1John", "1J;1. Jan;1jan"},
        {"2John", "2J;2. Jan;2jan"},
        {"3John", "3J;3. Jan;3jan"},
        {"Jude", "Ju;Juda;juda"},
        {"Rev", "Zj;Zjevení;zjeveni"}
      };
    }

    #endregion
  }
}