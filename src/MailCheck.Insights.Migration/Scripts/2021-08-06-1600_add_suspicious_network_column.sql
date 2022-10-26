ALTER TABLE `insights`.`aggregate_report_record_enriched`
ADD COLUMN `suspicious_network_blocklist_count` int(11) DEFAULT NULL AFTER `spf_pass_count`;