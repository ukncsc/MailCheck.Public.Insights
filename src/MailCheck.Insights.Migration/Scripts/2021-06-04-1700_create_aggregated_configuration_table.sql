CREATE TABLE IF NOT EXISTS `aggregated_configuration` (
  `domain` varchar(255) DEFAULT NULL,
  `reverse_domain` varchar(255) NOT NULL,
  `effective_date` date NOT NULL,
  `provider` varchar(255) NOT NULL,
  `policy` varchar(15) NOT NULL,
  `pct` bigint(20) NOT NULL,
  `all_traffic_count` bigint(20) DEFAULT NULL,
  `spf_failed_count` bigint(20) DEFAULT NULL,
  `dkim_failed_count` bigint(20) DEFAULT NULL,
  `spf_misaligned_count` bigint(20) DEFAULT NULL,
  `dkim_misaligned_count` bigint(20) DEFAULT NULL,
  `disposition_none_count` bigint(20) DEFAULT NULL,
  `disposition_quarantine_or_reject_count` bigint(20) DEFAULT NULL,
  `untrusted_count` bigint(20) DEFAULT NULL,
  `record_count` bigint(20) DEFAULT NULL,
  PRIMARY KEY (`reverse_domain`,`effective_date`,`provider`,`policy`,`pct`)
) ENGINE=InnoDB DEFAULT CHARSET=latin1;
