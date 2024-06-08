using Microsoft.Extensions.Diagnostics.HealthChecks;
using System.Text;


namespace HealthCheck.Models.HTML;

/// <summary>
/// Класс для загрузки и обработки HTML шаблонов
/// </summary>
internal sealed class HtmlTemplate
{
    private HealthReport? _report = null;

    private readonly string _mainStyle = "*,*::after,*::before{margin:0;padding:0;box-sizing:border-box}body{font-family:sans-serif}.title{text-align:left;margin:5px 0px;}.head{display:flex;align-content:center;justify-content:center;align-items:center;}.version{color:red;font-size:16px;text-align:end;margin-left:20px;margin-top:5px;}";
    private readonly string _accordionStyle = ".accordion{position:relative;margin:5px;}.accordion__checkbox{position:absolute;left:0;top:0}.accordion__title{background:#061e33;color:white;padding:7px;position:relative}.accordion__title-text::before{content:\"\";display:inline-block;border:10px solid transparent;border-left:15px solid white;}.accordion__title-text{cursor:pointer;display:flex;align-items:center;text-align:left}.accordion__description{display:block;cursor:pointer;font-size:17px}.accordion__description_success{color:#a5fba5}.accordion__description_partial{color:#f69c00}.accordion__description_error{color:#e90019}.accordion__content{background-color:white;box-shadow:0 0 10px 0 rgba(0,0,0,0.2);display:none;padding:5px}.accordion__checkbox:checked ~ .accordion__title label span::before{border-left:10px solid transparent;border-top:14px solid white;margin-top:7px;margin-right:3px;margin-left:2px}.accordion__checkbox:checked ~ .accordion__content{display:block}";
    private readonly string _tableStyle = ".container{max-width:1000px;margin-left:auto;margin-right:auto;padding-left:10px;padding-right:10px;}h2{font-size:26px;margin:20px 0;text-align:center;}h2 small{font-size:0.5em}.responsive-table li{border-radius:3px;padding:10px;display:flex;justify-content:space-between;margin-bottom:7px;}.responsive-table .table-header{background-color:#95A5A6;font-size:14px;text-transform:uppercase;letter-spacing:0.03em}.responsive-table .table-row{box-shadow:0 0 9px 0 rgba(0,0,0,0.1)}.responsive-table .col-1{flex-basis:5%}.responsive-table .col-2{flex-basis:35%;margin:auto}.responsive-table .col-3{flex-basis:35%;margin:auto}@media all and (max-width:767px){.responsive-table .table-header{display:none} .responsive-table .table-row{} .responsive-table li{display:block} .responsive-table .col{flex-basis:100%} .responsive-table .col{display:flex;padding:10px 0} .responsive-table .col:before{color:#6C7A89;padding-right:10px;content:attr(data-label);flex-basis:50%;text-align:right;margin:auto 0}}.table-row__success{background:#c3e6cb}.table-row__partial{background:#f69c00}.table-row__error{background:#f5c6cb}.table-row__success_icon{display:block;width:30px;height:30px;background-image:url(\"data:image/png;base64,iVBORw0KGgoAAAANSUhEUgAAAB4AAAAeCAYAAAA7MK6iAAAABmJLR0QA/wD/AP+gvaeTAAACpUlEQVRIidXXS2tVVxQH8F801UjTWxO5aAIOolAcFMGBIo4KPhIHVhwWRRH9CFLjREEQo1/CRyYdlE7UtpSAOOmo7aTpoFBawSoSkPhARWuNg7WOOR6Se89NFPEPm7vv2muv/36stfY6fADoxUD2GxjP1kjZYOrUQldNvX78jjVJNoSPc+wJ/sEB3MXnmG5ncEmLsZ40Noyv09i2XMR97M42jZXYmv3jGMm5PTU39gbOYypJnmBPjTlf4nHOmUobHWMCo+I6Wp1MFV3ZRtNGLawQq5zAU+zsgLCKXWljIm22PPYD4ohGF0laYGfamsJXrRRHxP3U9fY66BJOt6ssLN9dH77AR2+JuCmOd5m4wu0iIt7AJ7iNP9Tz3lbYiovYUZLtwST+VUkyg/gfmxdBOIRvRDL5bI7xzckxUB24iCsLJD2ER7iJtfPoXMWF4k93/jZyxQ9Lir04IvzgV/yMFxVjXTiDE7gnHOhWiwWuw6d4UAjG03izotiP7zAj8vAxLC2Nn8qxFypeOwea+CW5XmM8hVXiAmNJMCN2Pihy+MuU1UmNTbG5y2VhAze0vuOTJfK/ss3gTxEu7XAtORrM3vFD4Y2rWkw8LcLuGNanbAZHRWpsh5fJUfaj2uG0FNfN7vzbGoQFinBaUxYWCWRS+wSyXjx9/2FDTdK9IjndMltAvEY/zqXRds/gPhyuSbocz4SD9s2ntNu7eyRGysLuilI/novy5Tf8tEjSYWwS1zLvbonX5Jy3UwgMmy0ExnRQf5VLnzpxWmCFBZQ+ZZSLvWeikGuHvalbFHtjCyHuwX5xZGdFOGzB9/hBpMAmfkzZltQ5m3P2C4+eE3W9t08U9AMi164TsU88h3/jIO5goxoFfSfoxersN3ApW/EJs1oHnzDvDa8ACUuUqp/wb8cAAAAASUVORK5CYII=\")}.table-row__partial_icon{display:block;width:30px;height:30px;background-image:url(\"data:image/svg+xml;base64,PD94bWwgdmVyc2lvbj0iMS4wIiA/Pjxzdmcgdmlld0JveD0iMCAwIDMyIDMyIiB4bWxucz0iaHR0cDovL3d3dy53My5vcmcvMjAwMC9zdmciPjxkZWZzPjxzdHlsZT4uY2xzLTF7ZmlsbDojMTAxODIwO308L3N0eWxlPjwvZGVmcz48dGl0bGUvPjxnIGRhdGEtbmFtZT0iTGF5ZXIgNTgiIGlkPSJMYXllcl81OCI+PHBhdGggY2xhc3M9ImNscy0xIiBkPSJNMTYsMjZhMiwyLDAsMSwxLDItMkEyLDIsMCwwLDEsMTYsMjZabTAtMloiLz48cGF0aCBjbGFzcz0iY2xzLTEiIGQ9Ik0xNiwyMGExLDEsMCwwLDEtMS0xVjExYTEsMSwwLDAsMSwyLDB2OEExLDEsMCwwLDEsMTYsMjBaIi8+PHBhdGggY2xhc3M9ImNscy0xIiBkPSJNMjcuNzgsMzBINC4yMmEzLjE5LDMuMTksMCwwLDEtMi43Ny0xLjU3LDMuMTMsMy4xMywwLDAsMS0uMDYtMy4xM0wxMy4xNywzLjY3YTMuMjMsMy4yMywwLDAsMSw1LjY2LDBMMzAuNjEsMjUuM2EzLjEzLDMuMTMsMCwwLDEtLjA2LDMuMTNBMy4xOSwzLjE5LDAsMCwxLDI3Ljc4LDMwWk0xNiw0YTEuMTgsMS4xOCwwLDAsMC0xLjA3LjYzTDMuMTUsMjYuMjVhMS4xMiwxLjEyLDAsMCwwLDAsMS4xNiwxLjE5LDEuMTksMCwwLDAsMSwuNTlIMjcuNzhhMS4xOSwxLjE5LDAsMCwwLDEtLjU5LDEuMTIsMS4xMiwwLDAsMCwwLTEuMTZMMTcuMDcsNC42M0ExLjE4LDEuMTgsMCwwLDAsMTYsNFoiLz48L2c+PC9zdmc+\")}.table-row__error_icon{display:block;width:30px;height:30px;background-image:url(\"data:image/png;base64,iVBORw0KGgoAAAANSUhEUgAAAB4AAAAeCAYAAAA7MK6iAAAABmJLR0QA/wD/AP+gvaeTAAAEXklEQVRIie2WzY9URRTFf7fqtc1AFLbGsDJEYZiBuFDjmoX4N+BKBkYJmZAJMS5R8GOhgEKMC+N/ITtYKIRRHMQYjTGuRJdGNEx3v7ofLup1M5kmMGtDJfU6qa73zrnnnrq34NH4vw/ZuPDlwYP9XREXc87P5l7PRcSBCIgE4RARMfWRABF3iQgJSO4mUTQb8fNvuTn2yqVLo/XvNBuBd7Xtxbyl/1pvZobUZCTnewyjPiKgggeBIIB06x4OQJjiTYOovvT0cARw+IHA5j7XSCLlTK/fR3JGUqqfFyEiEBmTgJBODzoyEeCOqeKqlOEIb8vcRpwp4ASegCRClkTKDSnnGlVKeEQFkHVZiiB1XMIddx9HQep23Adnw3CPiKhAIqTtT7D12DGavXvrCynRrJs5JZqcSSnRm5tj2/HjNDt2kCQhIqzLy4OB3T3CDdxxVWYOvUqenWXLkSM0s7Nghndyjme41z0LC/X30CHC7F70pfhDgcU9xBzMEHfay5dBFZqG/sICvT17EFWiFEIVzOjt2UN/YQGaBlQpV64gbqCKlFIJPgw4zAI3QpVQpb25yt3zHxNtIXJD7+giaXZvJaNKeuZZegtHiNwQRVm7cJF2dbVGrIqb4aVsQupSwlQnrgxV9PtVBufOEm1L5Ex/cRGZm0N276b/xhtE0xClMLjwCXrj26pGKbgpXpWZAp5yNe6IO2JVqhAIhPLdd8S5s2xZOgG9Hv3Xj1WFmh60heH5s+jNmxBBuBNaoO3SUcrDpaaTKMbAbVsjLwVdWWHwwfvYaITnXGfbMvzoQ3RlBdoWb9txlFVuU2za1NMRV8dGzU/OlVl01aojZmpIyh3RWizMrJbO7py7GrgRHjWITUXsXk2hNUfeFqwUZN9+muWTRNNgo1GNPCXy0gny/D6slLpfFTfF1CYm2xywVceOne2mpLl5ZGkZk0wZtgzfe5fh6bcpa0NMMnLiJLL/OdyMKKW63pRwwyOmmtH9KpdgtShMjszeOVh+E0uZMhjSvvsOdu1r7JsV2jOn0MEARWBpGdn/XCWrCmqIB3lT5zhCIpwwq+zn9+HLb1FIlMGAcuYUevWrSRps5TrtmbcpHbgvLRPz+0GNWgGN2EzE4S7hY2BFDxykRKKsDdHTp4jrV+t/2uVSFV+5hp0+RVkbUiLhB17GTatXanmdAs4bF17fvv1w7jU707i737qJPvkU8sVncON6rb3mhFcT1hnw5+/4r7+g/a1w8SyxdndiUGvbPz69c+fz9ThTTG7t3Hn9sa0zL6Tc0IiAdJu8Nv7JuZq0xWCySbqeXa8FhDnFDF1bW5m/ffvF9TjT51hVrLSEGi7d7UKC8DHLmOpyIuM7CEiSyk0gPDA3zHQqpdPAo8GPo/DncwcaY8nXXXvWSzWhIPUhAtH14aAWJB+Ofngo8D9//X105vFtFIvdKSXvhK23OPfYWIMS4EBOiajI9bInpGQuZPlp7d+7ixtxHo3///gPp+xKtYTBHZ4AAAAASUVORK5CYII=\")}.version{color:red;font-size:16px;text-align:end}";

    /// <summary>
    /// Конструктор для шаблона
    /// </summary>
    /// <param name="report">Отчёт о работе приложения</param>
    public HtmlTemplate(HealthReport report)
    {
        _report = report;
    }

    /// <summary>
    /// Возвращает код HTML страницы
    /// </summary>
    public string RenderHtml()
    {
        if (_report == null)
        {
            return "Отсутствует отчёт о работе приложения";
        }

        var head = CreateHead();
        var body = CreateBody(_report);

        return
            "<html>" +
                $"{head}" +
                $"{body}" +
            "</html>";
    }

    /// <summary>
    /// Создание тега head
    /// </summary>
    /// <remarks>
    /// Иконку не подставляем, возьмёт из корня приложения стандартную - для всего приложения
    /// </remarks>
    private string CreateHead()
    {
        return
            "<head>" +
                "<meta charset='utf-8' />" +
                "<meta name='viewport' content='width=device-width, initial-scale=1' />" +
                "<title>Health Checks</title>" +
                "<meta name='description' content='The page checks the operation of the application'/>" +
                "<meta name='author' content='MUP Vodokanal'/>" +
                $"<style>{_mainStyle}</style>" +
                $"<style>{_accordionStyle}</style>" +
                $"<style>{_tableStyle}</style>" +
            "</head>";
    }

    /// <summary>
    /// Генерация тега body
    /// </summary>
    /// <param name="report">Результаты проверки работоспособности приложения</param>
    private string CreateBody(HealthReport report)
    {
        var bodyBuilder = new StringBuilder();
        bodyBuilder.AppendLine("<body>");

        bodyBuilder.AppendLine(
            $"<div class='head'>" +
                $"<h2 class='title'>Запрос состояния приложения</h2>" +
            $"</div>"
        );

        var rowIndex = 1;
        foreach (var keyValuePair in report.Entries)
        {
            var accordionContent = CreateAccordionContent(keyValuePair.Value);

            var accordion = CreateAccordion(rowIndex, keyValuePair.Key, accordionContent, keyValuePair.Value);

            bodyBuilder.AppendLine(accordion);

            rowIndex++;
        }

        return bodyBuilder.ToString();
    }

    /// <summary>
    /// Создание наполнения для "аккордеона"
    /// </summary>
    /// <param name="reportEntry">Результат проверки модуля</param>
    private string CreateAccordionContent(HealthReportEntry reportEntry)
    {
        const string firstCol = "col col-1";
        const string secondCol = "col col-2";
        const string thirdCol = "col col-3";

        var checksBuilder = new StringBuilder();
        checksBuilder.AppendLine("<ul class='responsive-table'>");
        checksBuilder.AppendLine(
            "<li class='table-header'>" +
                $"<div class='{firstCol}'>Check</div>" +
                $"<div class='{secondCol}'>Проверка</div>" +
                $"<div class='{thirdCol}'>Комментарий</div>" +
            "</li>");
        checksBuilder.AppendLine("<tbody>");

        foreach (var keyValuePair in reportEntry.Data)
        {
            var classTableRow = "table-row__success";
            var classIcon = "table-row__success_icon";

            var title = keyValuePair.Key;
            var comment = "";

            if (keyValuePair.Value is Check check)
            {
                comment += $"{check.Description} <br/>";

                if (check.CheckStartTime != null)
                {
                    comment += $"Старт: {check.CheckStartTime} <br/>";
                }

                if (check.CheckEndTime != null)
                {
                    comment += $"Конец: {check.CheckEndTime}";
                }

                switch (check.Status)
                {
                    case Enum.CheckStatus.Partially:
                        classTableRow = "table-row__partial";
                        classIcon = "table-row__partial_icon";
                        break;

                    case Enum.CheckStatus.Error:
                        classTableRow = "table-row__error";
                        classIcon = "table-row__error_icon";
                        break;

                    default:
                        break;
                }
            }
            else
            {
                comment += keyValuePair.Value.ToString();
            }

            checksBuilder.AppendLine(
                $"<li class='table-row {classTableRow}'>" +
                    $"<div class='{firstCol}' data-label='Check'>" +
                        $"<span class='{classIcon}'></span>" +
                    $"</div>" +
                    $"<div class='{secondCol}' data-label='Проверка'>{title}</div>" +
                    $"<div class='{thirdCol}' data-label='Комментарий'>{comment}</div>" +
                $"</li>");
        }

        checksBuilder.AppendLine("</tbody>");
        checksBuilder.AppendLine("</ul>");

        return
            "<div class='accordion__content'>" +
                $"{checksBuilder}" +
            "</div>";
    }

    /// <summary>
    /// Создание элемента "аккордеона"
    /// </summary>
    /// <param name="rowIndex">Индекс строки - используется для открытия на фронте</param>
    /// <param name="title">Заголовок элемента</param>
    /// <param name="accordionContent">Наполнение для элемента</param>
    /// <param name="report">Результат проверки модуля</param>
    private string CreateAccordion(int rowIndex, string title, string? accordionContent, HealthReportEntry report)
    {
        var successCount = 0;
        var partialCount = 0;
        var errorCount = 0;
        var undefinedCount = 0;
        var allCount = 0;

        foreach (var keyValuePair in report.Data)
        {
            if (keyValuePair.Value is Check check)
            {
                switch (check.Status)
                {
                    case Enum.CheckStatus.Passed:
                        successCount++;
                        break;

                    case Enum.CheckStatus.Partially:
                        partialCount++;
                        break;

                    case Enum.CheckStatus.Error:
                        errorCount++;
                        break;

                    default:
                        break;
                }
            }
            else
            {
                undefinedCount++;
            }
            allCount++;
        }

        var titleBuilder = new StringBuilder();
        titleBuilder.AppendLine("<div class='accordion__title'>");

        // Добавление заголовка для "аккордеона"
        titleBuilder.AppendLine("<h2 class='title'>");
        titleBuilder.AppendLine($"<label for='id-{rowIndex}'>");

        titleBuilder.AppendLine($"<span class='accordion__title-text'>{title}</span>");

        // Добавление информации по пройденным проверкам
        if (successCount != 0)
        {
            titleBuilder.AppendLine($"<span class='accordion__description accordion__description_success'>Пройдено проверок {successCount}/{allCount}</span>");
        }

        // Добавление информации по частичном прохождению
        if (partialCount != 0)
        {
            titleBuilder.AppendLine($"<span class='accordion__description accordion__description_partial'>Частично пройдено {partialCount}/{allCount}</span>");
        }

        // Добавление информации о не пройденных проверках
        if (errorCount != 0)
        {
            titleBuilder.AppendLine($"<span class='accordion__description accordion__description_error'>Проверка не пройдена {errorCount}/{allCount}</span>");
        }

        // Добавление информации о неопределённых проверках
        if (undefinedCount != 0)
        {
            titleBuilder.AppendLine($"<span class='accordion__description'>Неопределенный статус проверки {undefinedCount}/{allCount}</span>");
        }

        titleBuilder.AppendLine("</label>");
        titleBuilder.AppendLine("</h2>");

        titleBuilder.AppendLine("</div>");

        return
            "<div class='accordion'>" +
                $"<input id='id-{rowIndex}' class='accordion__checkbox' type='checkbox' />" +
                $"{titleBuilder}" +
                $"{accordionContent}" +
            "</div>";
    }
}