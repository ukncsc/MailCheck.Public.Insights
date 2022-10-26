CREATE TABLE IF NOT EXISTS `aggregated_abuse` (
  `domain` VARCHAR(255) NOT NULL,
  `reverse_domain` VARCHAR(255) NOT NULL,
  `effective_date` DATE NOT NULL,
  `count` BIGINT NOT NULL,
  `flagged` BIGINT NOT NULL,
  `p` enum('none','quarantine','reject') NOT NULL,
  `pct` BIGINT,
  PRIMARY KEY (`effective_date`, `reverse_domain`, `p`, `pct`)
) ENGINE=InnoDB DEFAULT CHARSET=latin1;