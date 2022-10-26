CREATE TABLE `aggregated_subdomains` (
  `domain` varchar(255) DEFAULT NULL,
  `reverse_domain` varchar(255) NOT NULL,
  `effective_date` date NOT NULL,
  `all_traffic_count` bigint(20) DEFAULT NULL,
  `dkim_or_spf_failed_none_count` bigint(20) DEFAULT NULL,
  `dkim_or_spf_failed_quarantine_or_reject_count` bigint(20) DEFAULT NULL,
  `dkim_and_spf_failed_none_count` bigint(20) DEFAULT NULL,
  `dkim_and_spf_failed_quarantine_or_reject_count` bigint(20) DEFAULT NULL,
  `blocklist_or_failed_reverse_dns_count` bigint(20) DEFAULT NULL,
  `record_count` bigint(20) DEFAULT NULL,
  PRIMARY KEY (`reverse_domain`,`effective_date`)
) ENGINE=InnoDB DEFAULT CHARSET=latin1;
