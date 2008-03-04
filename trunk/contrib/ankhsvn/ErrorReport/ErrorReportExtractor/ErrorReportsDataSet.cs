using System.Data;
namespace ErrorReportExtractor {

    namespace ErrorReportsDataSetTableAdapters
    {
        public partial class ErrorReportsTableAdapter
        {
            public static ErrorReportsTableAdapter Create()
            {
                ErrorReportsTableAdapter adapter = new ErrorReportsTableAdapter();
                adapter.Connection = ConnectionScope.Connection;
                return adapter;
            }
        }

        public partial class MailItemsTableAdapter
        {
            public static MailItemsTableAdapter Create()
            {
                MailItemsTableAdapter adapter = new MailItemsTableAdapter();
                adapter.Connection = ConnectionScope.Connection;
                return adapter;
            }
        }

        public partial class StackTraceLinesTableAdapter
        {
            public static StackTraceLinesTableAdapter Create()
            {
                StackTraceLinesTableAdapter adapter = new StackTraceLinesTableAdapter();
                adapter.Connection = ConnectionScope.Connection;
                return adapter;
            }
        }

        public partial class ErrorReportItemsTableAdapter
        {
            public static ErrorReportItemsTableAdapter Create()
            {
                ErrorReportItemsTableAdapter adapter = new ErrorReportItemsTableAdapter();
                adapter.Connection = ConnectionScope.Connection;
                return adapter;
            }
        }

        public partial class QueriesTableAdapter
        {
            public static QueriesTableAdapter Create()
            {
                QueriesTableAdapter adapter = new QueriesTableAdapter();
                foreach ( IDbCommand command in adapter.CommandCollection )
                {
                    command.Connection = ConnectionScope.Connection;
                }
                return adapter;
            }
        }
    }
}

namespace ErrorReportExtractor {


    partial class ErrorReportsDataSet
    {
    }
}
