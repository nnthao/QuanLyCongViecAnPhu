IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[DuAnFile]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[DuAnFile](
        [Id] [int] IDENTITY(1,1) NOT NULL,
        [DuAnId] [int] NULL,
        [TenFile] [nvarchar](500) NULL,
        [DuongDan] [nvarchar](1000) NULL,
        [NguoiXoaId] [int] NULL,
        [NgayXoa] [datetime] NULL,
        [IsDeleted] [bit] NULL,
        CONSTRAINT [PK_DuAnFile] PRIMARY KEY CLUSTERED ([Id] ASC),
        CONSTRAINT [FK_DuAnFile_DuAn] FOREIGN KEY([DuAnId]) REFERENCES [dbo].[DuAn] ([Id])
    )
END

IF NOT EXISTS (SELECT * FROM [dbo].[DatabaseVersion] WHERE [Name] = '20260122232417')
BEGIN
    INSERT INTO [dbo].[DatabaseVersion] ([Name]) VALUES ('20260122232417')
END
