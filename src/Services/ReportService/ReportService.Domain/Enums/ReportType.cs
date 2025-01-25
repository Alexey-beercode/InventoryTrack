namespace ReportService.Domain.Enums;

public enum ReportType: int
{
    /// <summary>
    /// Отчет о состоянии склада (остатки, категории, стоимость, истекающие активы).
    /// </summary>
    StockState,

    /// <summary>
    /// Отчет о внутренних перемещениях материальных ценностей.
    /// </summary>
    Movements,

    /// <summary>
    /// Отчет о списании материальных ценностей.
    /// </summary>
    WriteOffs,

    /// <summary>
    /// История операций (поступления, перемещения, списания).
    /// </summary>
    Items,
}
