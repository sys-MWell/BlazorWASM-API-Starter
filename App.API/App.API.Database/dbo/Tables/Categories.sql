CREATE TABLE [dbo].[Categories] (
    [CategoryID]          INT           NOT NULL,
    [CategoryName]        VARCHAR (100) NULL,
    [CategoryDescription] VARCHAR (255) NULL,
    [CategoryType]        VARCHAR (100) NULL,
    PRIMARY KEY CLUSTERED ([CategoryID] ASC)
);

