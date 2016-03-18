﻿using System;
using System.IO;
using System.Threading.Tasks;

using UpkManager.Domain.Constants;
using UpkManager.Domain.Helpers;
using UpkManager.Domain.Models.Properties;
using UpkManager.Domain.Models.Tables;


namespace UpkManager.Domain.Models.Objects {

  public class DomainObjectBase : DomainUpkBuilderBase {

    #region Constructor

    public DomainObjectBase() {
      PropertyHeader = new DomainPropertyHeader();
    }

    #endregion Constructor

    #region Properties

    public DomainPropertyHeader PropertyHeader { get; }

    public ByteArrayReader AdditionalDataReader { get; private set; }

    #endregion Properties

    #region Domain Properties

    public int AdditionalDataOffset { get; private set; }

    public virtual bool IsExportable => false;

    public virtual bool IsViewable => false;

    public virtual ObjectType ObjectType => ObjectType.Unknown;

    public virtual string FileExtension => String.Empty;

    public virtual string FileTypeDesc => String.Empty;

    #endregion Domain Properties

    #region Domain Methods

    public virtual async Task ReadDomainObject(ByteArrayReader reader, DomainHeader header, DomainExportTableEntry export, bool skipProperties, bool skipParse) {
      if (!skipProperties) await PropertyHeader.ReadPropertyHeader(reader, header);

      if (reader.Remaining == 0) return;

      AdditionalDataOffset = export.SerialDataOffset + reader.CurrentOffset;

      AdditionalDataReader = await reader.Splice();
    }

    public virtual async Task SaveObject(string filename) {
      await Task.FromResult(1);
    }

    public virtual Stream GetObjectStream() {
      return null;
    }

    #endregion Domain Methods

    #region DomainUpkBuilderBase Implementation

    public override int GetBuilderSize() {
      BuilderSize = PropertyHeader.GetBuilderSize()
                  + AdditionalDataReader?.GetBytes().Length ?? 0;

      return BuilderSize;
    }

    public override async Task WriteBuffer(ByteArrayWriter Writer) {
      await PropertyHeader.WriteBuffer(Writer);

      await Writer.WriteBytes(AdditionalDataReader?.GetBytes());
    }

    #endregion DomainUpkBuilderBase Implementation

  }

}
