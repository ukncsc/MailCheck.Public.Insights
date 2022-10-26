GRANT SELECT, INSERT, UPDATE, DELETE ON `insights_entity` TO '{env}-insights-ent' IDENTIFIED BY '{password}';
GRANT SELECT, INSERT, UPDATE, DELETE ON `aggregate_report_record_enriched` TO '{env}-insights-data-saver' IDENTIFIED BY '{password}';
GRANT SELECT, INSERT, UPDATE, DELETE ON `aggregated_configuration` TO '{env}-insights-data-saver' IDENTIFIED BY '{password}';
GRANT SELECT, INSERT, UPDATE, DELETE ON `aggregated_abuse` TO '{env}-insights-data-saver' IDENTIFIED BY '{password}';
GRANT SELECT, INSERT, UPDATE, DELETE ON `aggregated_subdomains` TO '{env}-insights-data-saver' IDENTIFIED BY '{password}';

GRANT SELECT, INSERT, UPDATE, DELETE ON `aggregate_report_record_enriched` TO '{env}-insights-gen' IDENTIFIED BY '{password}';
GRANT SELECT, INSERT, UPDATE, DELETE ON `aggregated_configuration` TO '{env}-insights-gen' IDENTIFIED BY '{password}';
GRANT SELECT, INSERT, UPDATE, DELETE ON `aggregated_abuse` TO '{env}-insights-gen' IDENTIFIED BY '{password}';
GRANT SELECT, INSERT, UPDATE, DELETE ON `aggregated_subdomains` TO '{env}-insights-gen' IDENTIFIED BY '{password}';

GRANT SELECT ON `insights_entity` TO '{env}-insights-api' IDENTIFIED BY '{password}';
GRANT SELECT ON `insights_entity` TO '{env}_reports' IDENTIFIED BY '{password}';
GRANT SELECT ON `aggregate_report_record_enriched` TO '{env}-insights-api' IDENTIFIED BY '{password}';

GRANT SELECT INTO S3 ON *.* TO '{env}_reports' IDENTIFIED BY '{password}';
