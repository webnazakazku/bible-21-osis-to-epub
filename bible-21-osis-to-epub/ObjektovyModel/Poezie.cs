﻿namespace BibleDoEpubu.ObjektovyModel
{
  /// <summary>
  /// Poezie.
  /// </summary>
  /// <remarks>
  /// Odpovídá tagu lg.
  /// </remarks>
  internal class Poezie : CastTextu
  {
    #region Metody

    public override string PrevestNaHtml()
    {
      return "<p>" + base.PrevestNaHtml() + "</p>";
    }

    #endregion
  }
}