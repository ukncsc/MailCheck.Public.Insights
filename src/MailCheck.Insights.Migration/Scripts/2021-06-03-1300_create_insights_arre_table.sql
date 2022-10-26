﻿CREATE TABLE IF NOT EXISTS `aggregate_report_record_enriched` (
  `record_id` bigint(20) NOT NULL,
  `domain` varchar(255) NOT NULL,
  `effective_date` date NOT NULL,
  `reverse_domain` varchar(255) NOT NULL,
  `adkim` enum('r','s') DEFAULT NULL,
  `arc` bigint(20) DEFAULT NULL,
  `aspf` enum('r','s') DEFAULT NULL,
  `bounce_reflector_blocklist_count` bigint(20) DEFAULT NULL,
  `count` bigint(20) DEFAULT NULL,
  `disposition` varchar(255) DEFAULT NULL,
  `dkim` varchar(255) DEFAULT NULL,
  `dkim_auth_results` varchar(1000) DEFAULT NULL,
  `dkim_fail_count` bigint(20) DEFAULT NULL,
  `dkim_pass_count` bigint(20) DEFAULT NULL,
  `domain_from` varchar(255) DEFAULT NULL,
  `end_user_blocklist_count` int(11) DEFAULT NULL,
  `end_user_network_blocklist_count` int(11) DEFAULT NULL,
  `envelope_from` varchar(255) DEFAULT NULL,
  `envelope_to` varchar(255) DEFAULT NULL,
  `fo` varchar(255) DEFAULT NULL,
  `forwarded` bigint(20) DEFAULT NULL,
  `header_from` varchar(255) DEFAULT NULL,
  `hijacked_network_blocklist_count` bigint(20) DEFAULT NULL,
  `host_as_description` varchar(255) DEFAULT NULL,
  `host_country` varchar(255) DEFAULT NULL,
  `host_name` varchar(255) DEFAULT NULL,
  `host_org_domain` varchar(255) DEFAULT NULL,
  `host_provider` varchar(255) DEFAULT NULL,
  `host_source_ip` varchar(255) DEFAULT NULL,
  `local_policy` bigint(20) DEFAULT NULL,
  `mailing_list` bigint(20) DEFAULT NULL,
  `malware_blocklist_count` bigint(20) DEFAULT NULL,
  `organisation_domain_from` varchar(255) DEFAULT NULL,
  `other_override_reason` bigint(20) DEFAULT NULL,
  `p` varchar(255) DEFAULT NULL,
  `pct` bigint(20) DEFAULT NULL,
  `proxy_blocklist_count` bigint(20) DEFAULT NULL,
  `reporter_org_name` varchar(255) DEFAULT NULL,
  `report_id` varchar(255) DEFAULT NULL,
  `sampled_out` bigint(20) DEFAULT NULL,
  `sp` varchar(255) DEFAULT NULL,
  `spam_source_blocklist_count` bigint(20) DEFAULT NULL,
  `spf` varchar(255) DEFAULT NULL,
  `spf_auth_results` varchar(1000) DEFAULT NULL,
  `spf_fail_count` bigint(20) DEFAULT NULL,
  `spf_pass_count` bigint(20) DEFAULT NULL,
  `trusted_forwarder` bigint(20) DEFAULT NULL,
  PRIMARY KEY (`reverse_domain`,`effective_date`,`record_id`)
) ENGINE=InnoDB DEFAULT CHARSET=latin1;
