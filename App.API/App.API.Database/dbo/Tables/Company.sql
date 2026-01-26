CREATE TABLE [dbo].[Company] (
    [CompanyID]   INT           NOT NULL,
    [CompanyName] VARCHAR (100) NOT NULL,
    [ImageID]     INT           NULL,
    PRIMARY KEY CLUSTERED ([CompanyID] ASC),
    CONSTRAINT [fk_company_imageid] FOREIGN KEY ([ImageID]) REFERENCES [dbo].[Images] ([ImageID])
);

