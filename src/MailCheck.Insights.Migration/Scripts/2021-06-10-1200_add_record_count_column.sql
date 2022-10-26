ALTER TABLE `insights`.`aggregated_abuse` 
ADD COLUMN `record_count` BIGINT(20) NULL DEFAULT NULL AFTER `pct`;