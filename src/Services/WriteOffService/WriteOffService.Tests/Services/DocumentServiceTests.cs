using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Moq;
using WriteOffService.Application.DTOs.Response.Document;
using WriteOffService.Application.Exceptions;
using WriteOffService.Application.Interfaces.Services;
using WriteOffService.Application.Services;
using WriteOffService.Domain.Entities;
using WriteOffService.Domain.Interfaces.UnitOfWork;
using Xunit;

namespace WriteOffService.Tests.Services
{
    public class DocumentServiceTests
    {
        private readonly Mock<IUnitOfWork> _mockUnitOfWork;
        private readonly Mock<IMapper> _mockMapper;
        private readonly DocumentService _service;

        public DocumentServiceTests()
        {
            _mockUnitOfWork = new Mock<IUnitOfWork>();
            _mockMapper = new Mock<IMapper>();
            _service = new DocumentService(_mockUnitOfWork.Object, _mockMapper.Object);
        }

        [Fact]
        public async Task GetDocumentAsync_ShouldReturnDocument()
        {
            // Arrange
            var documentId = Guid.NewGuid();
            var document = new Document
            {
                Id = documentId,
                FileName = "test.txt",
                FileType = "text/plain",
                FileData = new byte[] { 1, 2, 3 }
            };

            _mockUnitOfWork.Setup(uow => uow.Documents.GetByIdAsync(documentId, It.IsAny<CancellationToken>()))
                           .ReturnsAsync(document);

            // Act
            var result = await _service.GetDocumentAsync(documentId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(document.FileName, result.FileName);
            _mockUnitOfWork.Verify(uow => uow.Documents.GetByIdAsync(documentId, It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task GetDocumentAsync_ShouldThrowExceptionIfDocumentNotFound()
        {
            // Arrange
            var documentId = Guid.NewGuid();

            _mockUnitOfWork.Setup(uow => uow.Documents.GetByIdAsync(documentId, It.IsAny<CancellationToken>()))
                           .ReturnsAsync((Document)null);

            // Act & Assert
            await Assert.ThrowsAsync<EntityNotFoundException>(() => _service.GetDocumentAsync(documentId));
        }

        [Fact]
        public async Task DeleteDocumentAsync_ShouldDeleteDocument()
        {
            // Arrange
            var documentId = Guid.NewGuid();
            var document = new Document { Id = documentId };

            _mockUnitOfWork.Setup(uow => uow.Documents.GetByIdAsync(documentId, It.IsAny<CancellationToken>()))
                           .ReturnsAsync(document);

            // Act
            await _service.DeleteDocumentAsync(documentId);

            // Assert
            _mockUnitOfWork.Verify(uow => uow.Documents.Delete(document), Times.Once);
            _mockUnitOfWork.Verify(uow => uow.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task GetDocumentInfoAsync_ShouldReturnDocumentInfo()
        {
            // Arrange
            var documentId = Guid.NewGuid();
            var document = new Document
            {
                Id = documentId,
                FileName = "test.txt",
                FileType = "text/plain"
            };

            var documentInfoDto = new DocumentInfoResponseDto
            {
                Id = documentId,
                FileName = "test.txt",
                FileType = "text/plain"
            };

            _mockUnitOfWork.Setup(uow => uow.Documents.GetByIdAsync(documentId, It.IsAny<CancellationToken>()))
                           .ReturnsAsync(document);

            _mockMapper.Setup(m => m.Map<DocumentInfoResponseDto>(document)).Returns(documentInfoDto);

            // Act
            var result = await _service.GetDocumentInfoAsync(documentId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(documentInfoDto.FileName, result.FileName);
            Assert.Equal(documentInfoDto.FileType, result.FileType);
            _mockUnitOfWork.Verify(uow => uow.Documents.GetByIdAsync(documentId, It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task GetByRequestIdAsync_ShouldReturnDocuments()
        {
            // Arrange
            var requestId = Guid.NewGuid();
            var documents = new List<Document>
            {
                new Document { Id = Guid.NewGuid(), FileName = "test1.txt", FileType = "text/plain" },
                new Document { Id = Guid.NewGuid(), FileName = "test2.txt", FileType = "application/pdf" }
            };

            var documentDtos = documents.Select(d => new DocumentInfoResponseDto
            {
                Id = d.Id,
                FileName = d.FileName,
                FileType = d.FileType
            });

            // Mock WriteOffRequest exists
            _mockUnitOfWork.Setup(uow => uow.WriteOffRequests.GetByIdAsync(requestId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new WriteOffRequest { Id = requestId });

            // Mock documents for the request
            _mockUnitOfWork.Setup(uow => uow.Documents.GetByWriteOffRequestIdAsync(requestId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(documents);

            _mockMapper.Setup(m => m.Map<IEnumerable<DocumentInfoResponseDto>>(documents)).Returns(documentDtos);

            // Act
            var result = await _service.GetByRequestIdAsync(requestId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Count());
            _mockUnitOfWork.Verify(uow => uow.Documents.GetByWriteOffRequestIdAsync(requestId, It.IsAny<CancellationToken>()), Times.Once);
            _mockUnitOfWork.Verify(uow => uow.WriteOffRequests.GetByIdAsync(requestId, It.IsAny<CancellationToken>()), Times.Once);
        }

    }
}
